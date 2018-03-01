//
//  ViewController.swift
//  spaceHackathon
//
//  Created by Orsolya Lukacs-Kisbandi on 28/02/2018.
//  Copyright © 2018 Orsolya Lukacs-Kisbandi. All rights reserved.
//

import UIKit
import SceneKit
import ARKit

//enum
enum GeometryType: Int{
    case box = 1
    case plane = 2
}

class ViewController: UIViewController, ARSCNViewDelegate {
    
    @IBOutlet var sceneView: ARSCNView!

    var pointList: [SCNVector3] = []
    var loadcar: Bool = true
    var carObject = SCNNode()
    var angle : Float = 0.0
    var counter: Int = 0
    
    lazy var statusViewController: StatusViewController = {
        return childViewControllers.lazy.flatMap({ $0 as? StatusViewController }).first!
    }()
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        // Set the view's delegate
        sceneView.delegate = self
        
        // Show statistics such as fps and timing information
        sceneView.showsStatistics = true
        
        // Create a new scene
        let scene = SCNScene()
        
        // Set the scene to the view
        sceneView.scene = scene
        sceneView.autoenablesDefaultLighting = true
        let tap = UITapGestureRecognizer(target: self, action: #selector(tappedOnScreen(recognizer:)))
        self.sceneView.addGestureRecognizer(tap)
        
        statusViewController.restartExperienceHandler = { [unowned self] in
            print("Done pressed")
            self.visualisePath()
            self.postPath()
        }
    }
    
    func makePostCall(id:Int,commandType:String,value:Float) {
        let todosEndpoint: String = "http://openppr.eu.ngrok.io/api/Commands/Issue/"
        guard let todosURL = URL(string: todosEndpoint) else {
            print("Error: cannot create URL")
            return
        }
        var todosUrlRequest = URLRequest(url: todosURL)
        todosUrlRequest.httpMethod = "POST"
        todosUrlRequest.addValue("application/json", forHTTPHeaderField: "Content-Type")
        todosUrlRequest.addValue("application/json", forHTTPHeaderField: "Accept")
        let newTodo: [String: Any] = ["Id": id, "CommandType": commandType, "Data": Int(value)]
        let jsonTodo: Data
        do {
            jsonTodo = try JSONSerialization.data(withJSONObject: newTodo, options: [])
            todosUrlRequest.httpBody = jsonTodo
        } catch {
            print("Error: cannot create JSON from todo")
            return
        }
        
        let session = URLSession.shared
        
        let task = session.dataTask(with: todosUrlRequest) {
            (data, response, error) in
            guard error == nil else {
                print("error calling POST on /todos/1")
                print(error!)
                return
            }
            guard let responseData = data else {
                print("Error: did not receive data")
                return
            }
            
            // parse the result as JSON, since that's what the API provides
            do {
                guard let receivedTodo = try JSONSerialization.jsonObject(with: responseData, options: []) as? [String: Any] else {
                                                                            print("Could not get JSON from responseData as dictionary")
                                                                            return
                }
                print("The todo is: " + receivedTodo.description)
                
                guard let todoID = receivedTodo["id"] as? Int else {
                    print("Could not get todoID as int from JSON")
                    return
                }
                print("The ID is: \(todoID)")
            } catch  {
                print("error parsing response from POST on /todos")
                return
            }
        }
        task.resume()
    }
    
    func postPath(){
        let pointCount = pointList.count
        
        for index in 1...pointCount-1{
            
            print("Posting item: ")
            print(index)
            
            let angle =  calculateAngle(pos1: pointList[index], pos2: pointList[index-1])
            let distance = getDistance(pos1: pointList[index], pos2: pointList[index-1])
            
            var degrees = angle * 180/Float.pi
            
            var dir =  "Right"
            
            if degrees < 0{
                dir = "Left"
                degrees *= -1
            }
            
            self.makePostCall(id: counter, commandType: dir, value: degrees)
            counter = counter+1
            self.makePostCall(id: counter, commandType: "Forward", value: Float(distance*100))
            counter = counter+1
            
        }
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        
        // Create a session configuration
        let configuration = ARWorldTrackingConfiguration()
        configuration.planeDetection = .horizontal
        // Run the view's session
        sceneView.session.run(configuration)
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        
        // Pause the view's session
        sceneView.session.pause()
    }
    
    @objc func tappedOnScreen(recognizer: UITapGestureRecognizer) {
        let sceneV = recognizer.view as! ARSCNView
        let touchLocation = recognizer.location(in: sceneV)
        
        let hitResult = sceneV.hitTest(touchLocation, types: ARHitTestResult.ResultType.existingPlaneUsingExtent)
        if !hitResult.isEmpty {
            guard let hitResult = hitResult.first else {
                return
            }
            self.addCircle(hitResult: hitResult)
        }
    }
    
    func addCircle(hitResult: ARHitTestResult) {
        let x = hitResult.worldTransform.columns.3.x
        let y = hitResult.worldTransform.columns.3.y
        let z = hitResult.worldTransform.columns.3.z
        if loadcar{
            
            carObject = addCar(x: x, y:y, z: z)
            self.sceneView.scene.rootNode.addChildNode(carObject)
            
            pointList.append(carObject.position)
            loadcar = false
        } else {
            let box = SCNSphere(radius: 0.03)
            let boxNode = SCNNode(geometry: box)
            
            boxNode.categoryBitMask = GeometryType.box.rawValue
            
            boxNode.geometry?.firstMaterial?.diffuse.contents = UIColor.blue
            boxNode.position = SCNVector3Make(x, y, z)
            
            self.sceneView.scene.rootNode.addChildNode(boxNode)
            
            pointList.append(boxNode.position)
        }
    }
    
    func calculateAngle(pos1:SCNVector3,pos2:SCNVector3) -> (Float)  {
        print("Position1:",pos1)
        print("Position2:",pos2)
        let xLen = pos1.x - pos2.x
        let yLen = pos1.y - pos2.y
        let zLen = pos1.z - pos2.z
        
        let len = sqrt(pow(xLen,2)+pow(yLen,2)+pow(zLen,2))
        
        var angle = atan2(1, 0) - atan2(pos2.z-pos1.z, pos2.x-pos1.x);
        
        if (angle < 0){
            angle = angle + 2 * (Float.pi);
        }
        
        if angle > (Float.pi){
            angle = angle - 2*(Float.pi)
        }
        
        return angle
        
    }
    
    func visualisePath(){
        
        let pointCount = pointList.count
        var animList: [SCNAction] = []
        
        for index in 1...pointCount-1{
            
            print("Visualize path")
            print(index)
            
            var angle = calculateAngle(pos1: pointList[index], pos2: pointList[index-1])
            
            if angle > (Float.pi){
                angle = angle - 2*(Float.pi)
            }
            
            let and =  CGFloat(angle)
            
            print(pointList[index])
            let move = SCNAction.move(to: pointList[index], duration: 1.5)
            
            
            
            let rot = SCNAction.rotateTo(x: 0, y: and, z: 0, duration: 0.5)
            animList.append(rot)
            animList.append(move)
        }
        let animSequence = SCNAction.sequence(animList)
        
        
        carObject.runAction(animSequence)
    }
    
    func addLine(pos1:SCNVector3,pos2:SCNVector3) -> SCNNode {
        let lineLength = getDistance(pos1: pos1, pos2: pos2)
        let midPoint = findMidPoint(pos1: pos1, pos2: pos2)
        
        let line = SCNCylinder(radius: 0.005, height: lineLength)
        let lineNode = SCNNode(geometry: line)
        
        
        lineNode.categoryBitMask = GeometryType.box.rawValue
        lineNode.geometry?.firstMaterial?.diffuse.contents = UIColor.red
        lineNode.eulerAngles.x = -.pi / 2
        lineNode.position = midPoint
        
        return lineNode
        
    }
    
    func getDistance(pos1:SCNVector3,pos2:SCNVector3) -> CGFloat{
        
        print("Position1:",pos1)
        print("Position2:",pos2)
        let xLen = pos1.x - pos2.x
        let yLen = pos1.y - pos2.y
        let zLen = pos1.z - pos2.z
        
        let len = sqrt(pow(xLen,2)+pow(yLen,2)+pow(zLen,2))
        print("------- Length : ",len)
        return CGFloat(len)
    }
    
    func findMidPoint(pos1:SCNVector3,pos2:SCNVector3) -> SCNVector3{
        let xLen = (pos1.x + pos2.x)/2
        let yLen = (pos1.y + pos2.y)/2
        let zLen = (pos1.z + pos2.z)/2
        return SCNVector3Make(xLen,yLen,zLen)
    }
    
    func createPlaneNode(anchor: ARPlaneAnchor) -> SCNNode {
        
        let plane = SCNBox(width: CGFloat(anchor.extent.x), height: CGFloat(anchor.extent.z), length: 0.002 , chamferRadius: 0)
        
        // SCNPlane(width: CGFloat(anchor.extent.x), height: CGFloat(anchor.extent.z))
        let lavaImage = UIImage(named: "mars")
        let lavaMaterial = SCNMaterial()
        lavaMaterial.diffuse.contents = lavaImage
        lavaMaterial.isDoubleSided = true
        
        plane.materials = [lavaMaterial]
        
        let planeNode = SCNNode(geometry: plane)
        planeNode.position = SCNVector3Make(anchor.center.x, 0 , anchor.center.z)
        planeNode.transform = SCNMatrix4MakeRotation(-Float.pi / 2, 1, 0, 0)
        planeNode.opacity = 0.7
        return planeNode
    }
    
    func addCar(x: Float = 0, y: Float = 0, z: Float = 0) -> SCNNode {
        // if object found return it, else draw a red circle
        guard let carScene = SCNScene(named: "rover.dae") else {
            print("Object not found!")
            return self.addObject(color: "red", x: x, y: y, z: z+0.8) }
        let carNode = SCNNode()
        let carSceneChildNodes = carScene.rootNode.childNodes
        
        for childNode in carSceneChildNodes {
            carNode.addChildNode(childNode)
        }
        
        carNode.position = SCNVector3(x, y, z)
        carNode.scale = SCNVector3(0.05, 0.05, 0.05)
        //        carNode.eulerAngles.y = -.pi / 2
        return carNode
    }
    
    func addObject(color:String,x:Float,y:Float,z:Float) -> SCNNode{
        // Changes color and position of object based on input (later add other things like characters animations information etc)
        let myObect = SCNNode()
        myObect.geometry = SCNBox(width: 0.05, height: 0.05, length: 0.05, chamferRadius: 0.025)
        if color=="red"{
            myObect.geometry?.firstMaterial?.diffuse.contents = UIColor.red
        }else if color=="blue"{
            myObect.geometry?.firstMaterial?.diffuse.contents = UIColor.blue
        } else{
            myObect.geometry?.firstMaterial?.diffuse.contents = UIColor.orange
        }
        
        myObect.position = SCNVector3(x,y,z)
        return myObect
    }
    
    func renderer(_ renderer: SCNSceneRenderer, didAdd node: SCNNode, for anchor: ARAnchor) {
        guard let planeAnchor = anchor as? ARPlaneAnchor else {return}
        
        let planeNode = createPlaneNode(anchor: planeAnchor)
        node.addChildNode(planeNode)
        
    }
    
    func renderer(_ renderer: SCNSceneRenderer, didUpdate node: SCNNode, for anchor: ARAnchor) {
        guard let planeAnchor = anchor as? ARPlaneAnchor else {return}
        
        node.enumerateChildNodes { (chileNode, _) in
            chileNode.removeFromParentNode()
        }
        
        let planeNode = createPlaneNode(anchor: planeAnchor)
        node.addChildNode(planeNode)
        
    }
    
    func renderer(_ renderer: SCNSceneRenderer, didRemove node: SCNNode, for anchor: ARAnchor) {
        guard let _ = anchor as? ARPlaneAnchor else {return}
        node.enumerateChildNodes { (childNode, _) in
            childNode.removeFromParentNode()
        }
    }
    
    func session(_ session: ARSession, didFailWithError error: Error) {
        // Present an error message to the user
        
    }
    
    func sessionWasInterrupted(_ session: ARSession) {
        // Inform the user that the session has been interrupted, for example, by presenting an overlay
        
    }
    
    func sessionInterruptionEnded(_ session: ARSession) {
        // Reset tracking and/or remove existing anchors if consistent tracking is required
        
    }
}
