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
            
//            self.postToServer(id: <#T##Float#>, commandType: <#T##String#>, distance: <#T##Float#>, angle: <#T##Float#>)
//            let myUrl = URL(string:"http://2ba35d57.ngrok.io/api/Commands/Issue/")!
////            let postString = "id=13&CommandType=Forward&Data=payload test"
//
//            let json: [String: Any] = ["id": "13",
//                                       "CommandType": "Forward",
//                                       "Data": "payload test" ]
//
//            let jsonData = try? JSONSerialization.data(withJSONObject: json)
//
////            request.httpBody = postString.data(using: .utf8)
//
//// ---------- Requests
//            var request = URLRequest(url: myUrl)
////            request.httpBody = postString.data(using: .utf8)
//            request.httpBody = jsonData
//            request.httpMethod = "POST"
//            let (_, _, error) = URLSession.shared.synchronousDataTask(urlrequest: request)
//            if let error = error {
//                print("Synchronous task ended with error: \(error)")
//            }
//            else {
//                print("Synchronous task ended without errors.")
//            }
        }

    }
    
    func postToServer(id:Int,commandType:String,value:Float){
        let myUrl = URL(string:"http://openppr.eu.ngrok.io/api/Commands/Issue/")!
        
//        let command: [String: Any] = ["Id": String(id),
//                                   "CommandType": commandType,
//                                   "Data": String(value) ]
//
//        let jsonCommand = try? JSONSerialization.data(withJSONObject: command)
        let parameters = ["Id": String(id), "CommandType": commandType, "Data": String(value)] as Dictionary<String, String>

//        let data = "{\"Id\":\(id),\"CommandType\":\"\(commandType)\",\"Data\":\"\(value)\"}"
        
//        print(data)
//        // Requests
        var request = URLRequest(url: myUrl)
//        let jsonCommand = try? JSONSerialization.data(withJSONObject: data, options: .prettyPrinted)
        
        request.setValue("application/json; charset=utf-8", forHTTPHeaderField: "Content-Type")

//        // request.httpBody = postString.data(using: .utf8)
//        request.httpBody = jsonCommand
        do {
            request.httpBody = try JSONSerialization.data(withJSONObject: parameters, options: .prettyPrinted) // pass dictionary to nsdata object and set it as request body
            
        } catch let error {
            print(error.localizedDescription)
        }
        
        request.httpMethod = "POST"
        
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")
        request.addValue("application/json", forHTTPHeaderField: "Accept")
        
        let (_, _, error) = URLSession.shared.synchronousDataTask(urlrequest: request)
        if let error = error {
            print("Synchronous task ended with error: \(error)")
        }
        else {
            
            print("Synchronous task ended without errors.")
        }
    }
    
    func postPath(){
        let pointCount = pointList.count
        
        for index in 1...pointCount-1{
            
            print("Posting item: ")
            print(index)
            
            let angle = calculateAngle(pos1: pointList[index], pos2: pointList[index-1])
            let distance = getDistance(pos1: pointList[index], pos2: pointList[index-1])
            
            let degrees = angle * 180/Float.pi
            self.postToServer(id: counter, commandType: "Right", value: degrees)
            counter = counter+1
            self.postToServer(id: counter, commandType: "Forward", value: Float(distance*100))
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
            let box = SCNSphere(radius: 0.01)
            let boxNode = SCNNode(geometry: box)
            
            //        boxNode.physicsBody = SCNPhysicsBody(type: SCNPhysicsBodyType.dynamic, shape: SCNPhysicsShape(geometry: box, options: nil))
            
            //takes an enum
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
        
        //angle = angle * 360 / (2*(Float.pi));

        if (angle < 0){
            angle = angle + 2 * (Float.pi);
        }

        return angle
        
    }
    
    
    
//    func executePath(pointList: [SCNVector3]){
//
//        let pointCount = pointList.count
//
//        for index in 0...pointCount-1{
//            //executeStage(pos1: pointList[index], pos2: pointList[index+1])
//        }
//    }
    
    func visualisePath(){
        
        let pointCount = pointList.count
        var animList: [SCNAction] = []
        
        //let carObject = addCar(x: pointList[0].x, y:pointList[0].y, z: pointList[0].z)
        
        
        for index in 1...pointCount-1{
            
            print("Visualize path")
            print(index)
            
            let angle = calculateAngle(pos1: pointList[index], pos2: pointList[index-1])
            
            let and =  CGFloat(angle)
            var yRot = CGFloat(carObject.rotation.y)
            
            print("yrot")
            print(yRot)
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
        
        let plane = SCNBox(width: CGFloat(anchor.extent.x), height: CGFloat(anchor.extent.z), length: 0.02 , chamferRadius: 0)
        
        // SCNPlane(width: CGFloat(anchor.extent.x), height: CGFloat(anchor.extent.z))
        let lavaImage = UIImage(named: "mars")
        let lavaMaterial = SCNMaterial()
        lavaMaterial.diffuse.contents = lavaImage
        lavaMaterial.isDoubleSided = true
        
        plane.materials = [lavaMaterial]
        
        let planeNode = SCNNode(geometry: plane)
        planeNode.position = SCNVector3Make(anchor.center.x, 0 , anchor.center.z)
        planeNode.transform = SCNMatrix4MakeRotation(-Float.pi / 2, 1, 0, 0)
        planeNode.opacity = 0.5
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

//extension URLSession {
//    func synchronousDataTask(with url: URL) -> (Data?, URLResponse?, Error?) {
//        var data: Data?
//        var response: URLResponse?
//        var error: Error?
//
//        let semaphore = DispatchSemaphore(value: 0)
//
//        let dataTask = self.dataTask(with: url) {
//            data = $0
//            response = $1
//            error = $2
//
//            semaphore.signal()
//        }
//        dataTask.resume()
//
//        _ = semaphore.wait(timeout: .distantFuture)
//
//        return (data, response, error)
//    }
//}

extension URLSession {
    func synchronousDataTask(urlrequest: URLRequest) -> (data: Data?, response: URLResponse?, error: Error?) {
        var data: Data?
        var response: URLResponse?
        var error: Error?
        
        let semaphore = DispatchSemaphore(value: 0)
        
        let dataTask = self.dataTask(with: urlrequest) {
            data = $0
            response = $1
            error = $2
            
            semaphore.signal()
        }
        dataTask.resume()
        
        _ = semaphore.wait(timeout: .distantFuture)
        
        return (data, response, error)
    }
}

