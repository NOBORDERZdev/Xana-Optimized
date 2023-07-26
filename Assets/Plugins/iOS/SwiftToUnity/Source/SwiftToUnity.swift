import Foundation
@objc public class SwiftToUnity: NSObject
{
    @objc public static let shared = SwiftToUnity()
    @objc public func UnityOnStart(num:Int)
    {
        //UnitySendMessage("Cube", "OnMessageReceived", "print");
        print("myapp open===");
//        let customURL = URL(string: "myapp://x-callback-url/translate?x-success=sourceapp://x-callback-url/acceptTranslation&x-source=SourceApp&x-error=sourceapp://x-callback-url/translationError&word=Hello")
//        let customURL = "targetapp://x-callback-url/translate?x-success=sourceapp://x-callback-url/acceptTranslation&x-source=SourceApp&x-error=sourceapp://x-callback-url/translationError&word=Hello"
         var myString = String(num)
        print("send value1=\(myString)")
        let customURL = "xanaliaapp://connect/\(myString)"
	//let customURL = "xanaliaapp://\(num)"
        //let escapedURLString = strURL.addingPercentEncoding( withAllowedCharacters: .urlAllowed)
        let url = URL.init(string: customURL)
              if UIApplication.shared.canOpenURL(url!) {
                print("myapp open 1=\(url)");
                //let systemVersion = UIDevice.current.systemVersion//Get OS version
                //if Double(systemVersion)! >= 10.0 {//10 or above versions
                  //print(systemVersion)
                  //UIApplication.shared.open(customURL!, options: [:], completionHandler: nil)
                //} else {
                  //UIApplication.shared.openURL(customURL!)
                //}
                //OR
                if #available(iOS 10.0, *) {
                    print("myapp open 2=\(url)")
                  UIApplication.shared.open(url!, options: [:], completionHandler: nil)
                } else {
                    print("myapp open 3=\(url)")
                  UIApplication.shared.openURL(url!)
                }
              } else {
 		UIApplication.shared.openURL(URL(string: "https://www.xanalia.com/")!)    
                print("myapp  not Present===");
                 //Print alert here
               // lunchLabel.text = "not Present"
             }
    }
}   
