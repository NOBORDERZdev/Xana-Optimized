    #import <UnityFramework/UnityFramework-Swift.h>

extern "C"
{
    void UnityOnStart(int a)
    {
        [[SwiftToUnity shared]   UnityOnStartWithNum:(a)];
    }
}
