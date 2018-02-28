//
//  ViewController.swift
//  spaceHackathon
//
//  Created by Orsolya Lukacs-Kisbandi on 28/02/2018.
//  Copyright © 2018 Orsolya Lukacs-Kisbandi. All rights reserved.
//

//import UIKit
//import SceneKit
//import ARKit
//import Vision
//import AVFoundation
//
//class ViewController: UIViewController, ARSCNViewDelegate, AVCaptureVideoDataOutputSampleBufferDelegate {
//
//    @IBOutlet var sceneView: ARSCNView!
//
////    override func viewDidLoad() {
////        super.viewDidLoad()
////
////        // Set the view's delegate
////        sceneView.delegate = self
////
////        // Show statistics such as fps and timing information
////        sceneView.showsStatistics = true
////
////        // Create a new scene
////        let scene = SCNScene()
////
////        // enable natural lighting
////        sceneView.autoenablesDefaultLighting = true
////
////        // create cube
//////        let circle = SCNSphere(radius: 0.1)
//////
//////        // create material
//////        let material = SCNMaterial()
//////        material.diffuse.contents = UIColor.blue
//////
//////        // create SCNNode
//////        let node = SCNNode(geometry: circle)
//////        node.geometry?.materials = [material]
//////        node.position = SCNVector3Make(0, -0.1, -0.8)
////
////        // add SCNNode to the scene
//////        scene.rootNode.addChildNode(node)
////
////        let tap = UITapGestureRecognizer(target: self, action: #selector(tappedOnScreen(recognizer:)))
////        self.sceneView.addGestureRecognizer(tap)
////
////        print(tap)
////
////        // Set the scene to the view
////        sceneView.scene = scene
////    }
//
//    override func viewDidLoad() {
//        super.viewDidLoad()
//
//        // Set the view's delegate
//        sceneView.delegate = self
//
//        // Show statistics such as fps and timing information
//        sceneView.showsStatistics = true
//
//        // Create a new scene
//        let scene = SCNScene()
//
//        // Set the scene to the view
//        sceneView.scene = scene
//        let tap = UITapGestureRecognizer(target: self, action: #selector(tappedOnScreen(recognizer:)))
//        self.sceneView.addGestureRecognizer(tap)
//    }
//
//    override func viewWillAppear(_ animated: Bool) {
//        super.viewWillAppear(animated)
//
//        // Create a session configuration
//        let configuration = ARWorldTrackingConfiguration()
//
//        // Run the view's session
//        sceneView.session.run(configuration)
//    }
//
//    @objc func tappedOnScreen(recognizer: UITapGestureRecognizer) {
//        print("Tapped")
//        let sceneV = recognizer.view as! ARSCNView
//        let touchLocation = recognizer.location(in: sceneV)
//        print(touchLocation)
//
//        let hitResult = sceneV.hitTest(touchLocation, types: ARHitTestResult.ResultType.existingPlaneUsingExtent)
//
//        if !hitResult.isEmpty {
//
//            guard let hitResult = hitResult.first else {
//                return
//            }
//            print("not empty")
//            self.addCircle(hitResult: hitResult)
//        }
//        print("empty but why?")
//    }
//
////    @objc func tappedOnScreen(recognizer: UITapGestureRecognizer) {
////        let sceneV = recognizer.view as! ARSCNView
////        let touchLocation = recognizer.location(in: sceneV)
////
////        let hitResult = sceneV.hitTest(touchLocation, types: ARHitTestResult.ResultType.existingPlaneUsingExtent)
////        if !hitResult.isEmpty {
////            guard let hitResult = hitResult.first else {
////                return
////            }
////            self.addCircle(hitResult: hitResult)
////        }
////    }
//
//    func addCircle(hitResult: ARHitTestResult) {
//        print("add the circle")
//        let circle = SCNSphere(radius: 0.1)
//        let node = SCNNode(geometry: circle)
//        // create material
//        let material = SCNMaterial()
//        material.diffuse.contents = UIColor.blue
//
//        // create SCNNode
//        node.geometry?.materials = [material]
//        node.position = SCNVector3Make(0, -0.1, -0.8)
//
////        boxNode.physicsBody = SCNPhysicsBody(type: SCNPhysicsBodyType.dynamic, shape: SCNPhysicsShape(geometry: box, options: nil))
//
//        node.position = SCNVector3Make(hitResult.worldTransform.columns.3.x, hitResult.worldTransform.columns.3.y + Float(circle.radius) + Float(1), hitResult.worldTransform.columns.3.z)
//        self.sceneView.scene.rootNode.addChildNode(node)
//    }
//
//    override func viewWillDisappear(_ animated: Bool) {
//        super.viewWillDisappear(animated)
//
//        // Pause the view's session
//        sceneView.session.pause()
//    }
//
//    override func didReceiveMemoryWarning() {
//        super.didReceiveMemoryWarning()
//        // Release any cached data, images, etc that aren't in use.
//    }
//
//    // MARK: - ARSCNViewDelegate
//
///*
//    // Override to create and configure nodes for anchors added to the view's session.
//    func renderer(_ renderer: SCNSceneRenderer, nodeFor anchor: ARAnchor) -> SCNNode? {
//        let node = SCNNode()
//
//        return node
//    }
//*/
//
//    func session(_ session: ARSession, didFailWithError error: Error) {
//        // Present an error message to the user
//
//    }
//
//    func sessionWasInterrupted(_ session: ARSession) {
//        // Inform the user that the session has been interrupted, for example, by presenting an overlay
//
//    }
//
//    func sessionInterruptionEnded(_ session: ARSession) {
//        // Reset tracking and/or remove existing anchors if consistent tracking is required
//
//    }
//}


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
        let tap = UITapGestureRecognizer(target: self, action: #selector(tappedOnScreen(recognizer:)))
        self.sceneView.addGestureRecognizer(tap)
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
    
    func executeStage(pos1:SCNVector3,pos2:SCNVector3)  {
        
    }
    
    
    
    func executePath(pointList: [SCNVector3]){
        
        let pointCount = pointList.count
        
        for index in 0...pointCount-1{
            executeStage(pos1: pointList[index], pos2: pointList[index+1])
        }
    }
    
    func visualisePath(pointList: [SCNVector3]){
        
        let pointCount = pointList.count
        
        
        let carObject = addCar(x: pointList[0].x, y:pointList[0].y, z: pointList[0].z)
        
        self.sceneView.scene.rootNode.addChildNode(carObject)
        
        for index in 1...pointCount-1{

            // let angle = carObject.position.angleBetweenVectors(boxNode.position)
            
            let target = SCNSphere(radius: 0.0002)
            let targetNode = SCNNode(geometry: target)
            
            
            
            let pointTarget = pointList[index] + pointList[index] - carObject.position
            targetNode.position = pointTarget
            
            let move = SCNAction.move(to: pointList[index], duration: 1.5)
            let animSequence = SCNAction.sequence([ move])
            
            let lookAt = SCNLookAtConstraint(target: targetNode)
            
            carObject.constraints = [lookAt]
            carObject.runAction(animSequence)
            
        }
    }
    
    
    func addLine(pos1:SCNVector3,pos2:SCNVector3) -> SCNNode {
        let lineLength = calcPositions(pos1: pos1, pos2: pos2)
        let midPoint = findMidPoint(pos1: pos1, pos2: pos2)

        let line = SCNCylinder(radius: 0.005, height: lineLength)
        let lineNode = SCNNode(geometry: line)
        

        lineNode.categoryBitMask = GeometryType.box.rawValue
        lineNode.geometry?.firstMaterial?.diffuse.contents = UIColor.red
        lineNode.eulerAngles.x = -.pi / 2
        lineNode.position = midPoint
        
        return lineNode

    }
    
    func calcPositions(pos1:SCNVector3,pos2:SCNVector3) -> CGFloat{

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
    
//    func addCircle(hitResult: ARHitTestResult) {
//        print("add the circle")
//        let circle = SCNSphere(radius: 0.1)
//        let node = SCNNode(geometry: circle)
//        // create material
//        let material = SCNMaterial()
//        material.diffuse.contents = UIColor.blue
//
//        // create SCNNode
//        node.geometry?.materials = [material]
//        node.position = SCNVector3Make(0, -0.1, -0.8)
//
//        //        boxNode.physicsBody = SCNPhysicsBody(type: SCNPhysicsBodyType.dynamic, shape: SCNPhysicsShape(geometry: box, options: nil))
//
//        node.position = SCNVector3Make(hitResult.worldTransform.columns.3.x, hitResult.worldTransform.columns.3.y + Float(circle.radius) + Float(1), hitResult.worldTransform.columns.3.z)
//        self.sceneView.scene.rootNode.addChildNode(node)
//    }
    
    func createPlaneNode(anchor: ARPlaneAnchor) -> SCNNode {
        
        let plane = SCNBox(width: CGFloat(anchor.extent.x), height: CGFloat(anchor.extent.z), length: 0.02 , chamferRadius: 0)
        
        // SCNPlane(width: CGFloat(anchor.extent.x), height: CGFloat(anchor.extent.z))
        let lavaImage = UIImage(named: "mars")
        let lavaMaterial = SCNMaterial()
        lavaMaterial.diffuse.contents = lavaImage
        lavaMaterial.isDoubleSided = true
        
        plane.materials = [lavaMaterial]
        
        let planeNode = SCNNode(geometry: plane)
        planeNode.position = SCNVector3Make(anchor.center.x, 0, anchor.center.z)
        planeNode.transform = SCNMatrix4MakeRotation(-Float.pi / 2, 1, 0, 0)
//        planeNode.opacity = 0.2
        return planeNode
    }
    
    func addCar(x: Float = 0, y: Float = 0, z: Float = 0) -> SCNNode {
        // if object found return it, else draw a red circle
        guard let carScene = SCNScene(named: "car.dae") else {
            print("Object not found!")
            return self.addObject(color: "red", x: x, y: y, z: z+0.8) }
        let carNode = SCNNode()
        let carSceneChildNodes = carScene.rootNode.childNodes
        
        for childNode in carSceneChildNodes {
            carNode.addChildNode(childNode)
        }
        
        carNode.position = SCNVector3(x, y, z)
        carNode.scale = SCNVector3(0.2, 0.2, 0.2)
//        carNode.eulerAngles.x = -.pi / 2
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

