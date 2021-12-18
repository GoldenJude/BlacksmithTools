## Notes
In case of questions you may find me on the [Modding discord server](https://discord.gg/MXqWrn532w)  
You can also file an issue on [GitHub](https://github.com/GoldenJude/BlacksmithTools/issues)  

## Features

#### Bone reoder
- Automatically fixes the order of bones in skinned mesh renderers for custom armors/capes/utility items etc., solving the issue of incorrectly deformed meshes as long as the mesh has the correct bone weights

#### Body hiding
- provides the ability to hide certain parts of the player model for easier armor creation

## Installation
Place the BlacksmithTools.dll into your Bepinex/plugins folder.

## Configuration

Body hiding is configured by specifying a combination of body part names and bone indexes
There are two ways to configure body hiding:
  
#### Config file
Create a file named bsmith.ITEM_PREFAB_NAME.cfg and launch once to generate fields  
- for the field "List" input a list of body parts names delimited by a semicolon. i.e. Head;Torso;ArmUpperLeft  
- for the field "Bone List" input a list of bone names delimited by a semicolon. i.e. 0;1;2;4;7;12;3  

#### Code
Access the BlacksmithTools.BodypartSystem class and insert your configuration into either of the two dictionaries with the key being the item's prefab name

#### Valid values for configuration via part names
    
    Head
    Torso
    ArmUpperLeft
    ArmLowerLeft
    HandLeft
    ArmUpperRight
    ArmLowerRight
    HandRight
    LegUpperLeft
    LegLowerLeft
    FootLeft
    LegUpperRight
    LegLowerRight
    FootRight

#### Values for configuration via bone indexes (list gained by printing out player model's bones array)

    Hips - 0
    Spine - 1
    Spine1 - 2
    Spine2 - 3
    Neck - 4
    Head - 5
    Jaw - 6
    LeftShoulder - 7
    LeftArm - 8
    LeftForeArm - 9
    LeftHand - 10
    LeftHandThumb1 - 11
    LeftHandThumb2 - 12
    LeftHandThumb3 - 13
    LeftHandIndex1 - 14
    LeftHandIndex2 - 15
    LeftHandIndex3 - 16
    LeftHandMiddle1 - 17
    LeftHandMiddle2 - 18
    LeftHandMiddle3 - 19
    LeftHandRing1 - 20
    LeftHandRing2 - 21
    LeftHandRing3 - 22
    LeftHandPinky1 - 23
    LeftHandPinky2 - 24
    LeftHandPinky3 - 25
    RightShoulder - 26
    RightArm - 27
    RightForeArm - 28
    RightHand - 29
    RightHandThumb1 - 30
    RightHandThumb2 - 31
    RightHandThumb3 - 32
    RightHandIndex1 - 33
    RightHandIndex2 - 34
    RightHandIndex3 - 35
    RightHandMiddle1 - 36
    RightHandMiddle2 - 37
    RightHandMiddle3 - 38
    RightHandRing1 - 39
    RightHandRing2 - 40
    RightHandRing3 - 41
    RightHandPinky1 - 42
    RightHandPinky2 - 43
    RightHandPinky3 - 44
    LeftUpLeg - 45
    LeftLeg - 46
    LeftFoot - 47
    LeftToeBase - 48
    RightUpLeg - 49
    RightLeg - 50
    RightFoot - 51
    RightToeBase - 52

## Changelog  
- **2.0.2**  
armor stand update - hiding does not apply to unreadable body models anymore  
added .cfg extension and made explanation of config files a bit clearer  
- **2.0.1**  
Error when equipping armors with disabled skinned mesh renderers (plate armor) should be fixed   
- **2.0.0**  
Added configuration to disable features separately  
Changed body hiding to alter the body mesh directly without having to supply my own mesh
