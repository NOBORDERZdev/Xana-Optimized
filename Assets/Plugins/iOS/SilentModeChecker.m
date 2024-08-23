#import <AVFoundation/AVFoundation.h>
void _OverrideSilentMode() {
    NSError *error = nil;
    [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayback error:&error];
    if (error != nil) {
        NSLog(@"Error setting AVAudioSession category: %@", error);
    }
    [[AVAudioSession sharedInstance] setActive:YES error:&error];
    if (error != nil) {
        NSLog(@"Error activating AVAudioSession: %@", error);
    }
}
bool _IsSilentModeActive() {
    AVAudioSession *audioSession = [AVAudioSession sharedInstance];
    return (audioSession.category == AVAudioSessionCategorySoloAmbient);
}
