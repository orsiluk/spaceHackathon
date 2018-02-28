//
//  RobotInterface.swift
//  spaceHackathon
//
//  Created by John Griffith on 28/02/2018.
//  Copyright © 2018 Orsolya Lukacs-Kisbandi. All rights reserved.
//

import Foundation
//import Alamofire

class RobotInterface{
    
//    func syncRequest(){
//        var request = NSMutableURLRequest(URL: NSURL(string: "YOUR URL"))
//        var session = NSURLSession.sharedSession()
//        request.HTTPMethod = "POST"
//        
//        var params = ["username":"username", "password":"password"] as Dictionary<String, String>
//        
//        var err: NSError?
//        request.HTTPBody = NSJSONSerialization.dataWithJSONObject(params, options: nil, error: &err)
//        request.addValue("application/json", forHTTPHeaderField: "Content-Type")
//        request.addValue("application/json", forHTTPHeaderField: "Accept")
//        
//        var task = session.dataTaskWithRequest(request, completionHandler: {data, response, error -> Void in
//            println("Response: \(response)")})
//        
//        task.resume()
//    }
//    func postDataSynchronous(url: String, bodyData: String, completionHandler: (_ responseString: String?, _ error: NSError?) -> ())
//    {
//        let URL: NSURL = NSURL(string: url)!
//        var request:NSMutableURLRequest = NSMutableURLRequest(url:URL as URL)
//        request.httpMethod = "POST"
//        request.httpBody = bodyData.data(using: String.Encoding.utf8);
//        request.addValue("application/x-www-form-urlencoded", forHTTPHeaderField: "Content-Type")
//
//        var response: URLResponse?
//        var error: NSError?
//
//        // Send data
//        let data = NSURLConnection.sendSynchronousRequest(request, returningResponse: &response, error: &error)
//
//        var output: String! // Default to nil
//
//        if data != nil{
//            output =  NSString(data: data!, encoding: NSUTF8StringEncoding) as! String
//        }
//
//        completionHandler(responseString: output, error: error)
//
//    }
    
//    let queue = DispatchQueue(label: "com.cnoon.response-queue", qos: .utility, attributes: [.concurrent])
//
//    let endpoint  = "http://localhost:50937/api/commands/Issue/"
//
//    func moveForward(duration: Int32)  {
//
//        let body : Parameters = [ "id " : "5", "CommandType": "Forward" , "Data": "payload test" ];
//
//        Alamofire.request(self.endpoint, method: .post , parameters: body ).responseJSON { response in
//
//            print("Request: \(String(describing: response.request))")   // original url request
//            print("Response: \(String(describing: response.response))") // http url response
//            print("Result: \(response.result)")                         // response serialization result
//
//            if let json = response.result.value {
//                print("JSON: \(json)") // serialized json response
//            }
//
//            if let data = response.data, let utf8Text = String(data: data, encoding: .utf8) {
//                print("Data: \(utf8Text)") // original server data as UTF8 string
//            }
//
//        }
//    }
//
//    func threadTest()
//    {
//        DispatchQueue.global(qos: .userInitiated).async {
//            // Download file or perform expensive task
//
//            DispatchQueue.main.async {
//                // Update the UI
//            }
//        }
//    }
    
    
}



