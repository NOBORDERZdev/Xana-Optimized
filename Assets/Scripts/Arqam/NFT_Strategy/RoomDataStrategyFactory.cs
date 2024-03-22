using System;

public static class RoomDataStrategyFactory
{
    public static IRoomDataStrategy CreateRoomDataStrategy(string projectType)
    {
        // Depending on the project type, return appropriate strategy
        switch (projectType)
        {
            //case "ProjectTypeA":
            //    return new ApiRoomDataStrategy(); // or any other strategy for ProjectTypeA
            //case "ProjectTypeB":
            //    return new ModifiedApiRoomDataStrategy(); // or any other strategy for ProjectTypeB
            default:
                throw new ArgumentException("Invalid project type");
        }
    }

}
