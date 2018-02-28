//
//  RobotInterface.swift
//  spaceHackathon
//
//  Created by John Griffith on 28/02/2018.
//  Copyright © 2018 Orsolya Lukacs-Kisbandi. All rights reserved.
//

import Foundation
import Alamofire

class RobotInterface {
    
    let queue = DispatchQueue(label: "com.cnoon.response-queue", qos: .utility, attributes: [.concurrent])
    
    let endpoint  = "http://localhost:50937/api/commands/Issue/"
    
    func moveForward(duration: Int32)  {
        
        let body : Parameters = [ "id " : "5", "CommandType": "Forward" , "Data": "payload test" ];
        
        Alamofire.request(self.endpoint, method: .post , parameters: body ).responseJSON { response in
            
            print("Request: \(String(describing: response.request))")   // original url request
            print("Response: \(String(describing: response.response))") // http url response
            print("Result: \(response.result)")                         // response serialization result
            
            if let json = response.result.value {
                print("JSON: \(json)") // serialized json response
            }
            
            if let data = response.data, let utf8Text = String(data: data, encoding: .utf8) {
                print("Data: \(utf8Text)") // original server data as UTF8 string
            }
            
        }
    }
    
    func threadTest()
    {
        DispatchQueue.global(qos: .userInitiated).async {
            // Download file or perform expensive task
            
            DispatchQueue.main.async {
                // Update the UI
            }
        }
    }
    
    
}



