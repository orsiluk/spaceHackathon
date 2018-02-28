/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Utility class for showing messages above the AR view.
*/

import Foundation
import ARKit

class StatusViewController: UIViewController {

    
    @IBOutlet weak private var restartExperienceButton: UIButton!

    
    /// Trigerred when the "Restart Experience" button is tapped.
    var restartExperienceHandler: () -> Void = {}
  
    @IBAction private func restartExperience(_ sender: UIButton) {
        restartExperienceHandler()
    }
	
}

