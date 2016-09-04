# The EDVArd Project

## Introduction
Frontier Development's **Elite Dangerous** game is the 21st Century version of David Braben's and Ian Bell's classic *space simulator* from 1984. In summary, players are offered the freedom to explore an entire galaxy and to contribute to its sophisticated socio-economic model with options to explore, trade, mine or fight as a 31st Century space-pilot. Part of the route to space-pilot immersion is achieved by providing the player with a comprehensive and almost overwhelming array of bindable controls. These controls can either be bound to joystick, keyboard or both. If a player is lucky enough to be able to play the game with both keyboard and HOTAS joystick, he is well provided for with some default configurations. Although that might be satisfactory for some, for others it is not, and many weeks can be spent fine tuning what game action gets bound to which control.

## The Road to Immersion
VoiceAttack *http://voiceattack.com* in conjunction with Microsoft's Speech Recognition engine provides the capability of being able to interact with a PC by effectively allowing a person's voice to be used as an extra controller to simulate simple, single key presses from keyboard and/or mouse to complex macros where the player can be provided with audio feedback for his spoken commands.

HCSVoicePacks *http://www.hcsvoicepacks.com* has cleverly added to VoiceAttack's capability by creating game-specific content by which the player is invited not to only don his joystick, keyboard and headset, but also his microphone. The end result is an even deeper immersion with the player able to issue instruction by voice and having the ship's computer perform that instruction and provide an audio response. HCS not only provide all the audio, but they also provide a number of VoiceAttack profiles (.vap) in which voice commands and their audio responses have been mapped to Elite Dangerous keys ready for import into VoiceAttack.

The experience of playing Elite Dangerous  using these combined technologies is greatly enhanced when playing with regular monitor or projected displays. 

## The Challenge
The use of voice control becomes even more significant for those of us lucky enough to play this game with a head-mounted display.
The description above is great, but there is a challenge. VoiceAttack makes use of a *"VoiceAttack profile"* in which all the mappings between spoken and other actions are contained along with which buttons are to be actuated. If a player hasn't deviated from the default bindings, then chances are most, if not all, voice commands will work correctly as HCS do go to great pains to provide at least a few profiles to support common setups. If like me however, you have gone down the route of customising your Elite Dangerous key and Joystick bindings, then chances are the profiles offered by HCS will not work "out of the box". Your only solution is to manually go through a provided profile (or create your own) and either change your Elite Key Bindings to match Voice Attack's or change Voice Attack's. This can be quite cumbersome if you like to use multiple .binds and switch between different VoicePacks.

Until now ...

## The Solution
Project EDVArd (**E**lite **D**angerous **V**oice **A**ttack **r**ea**d**er) is a simple command-line utility that allows you to instantly synchronise your Elite Dangerous custom bindings with a selected VoiceAttack profile saving you time and effort.

### Basic Premise
1. A player's  Elite Dangerous .binds and VoiceAttack profile files are both analysed. 
2. Should a difference be found with some of the action to key binding mappings within the VoiceAttack profile file, they can be   
   synchronised to match those defined in the Elite Dangerous .binds. 
3. The Elite Dangerous .binds file is treated as sacrosanct. Key binding(s) that already exist are not affected. However, should any vacant binding slots exist for a defined Voice Attack Command , then an option to synchronise to the .binds file is provided.

## Usage
So how is it used?

Hopefully, the following set of instructions can help.

1. Download the single *EDVArd.exe* (this has been virus checked, and at current version (1.000) has a MD5 CRC/Checksum of **EF951712CB7386DA57DC2DDD2C85E696**)
2. Note where your Elite Dangerous Bindings .binds file is (it's usually in: **%LOCALAPPDATA%\Frontier Developments\Elite Dangerous\Options\Bindings**)
3. Note where your VoiceAttack Profile of choice is (a few can usually be found in: **%ProgramFiles(x86)%\VoiceAttack\Sounds\hcspack\Profiles** )
4. Open a command window (Dos or PowerShell, EdVard will work in either) to where you downloaded EdVard in
5. Examine the options available and examples provided by typing **EdVArd** or **EdVArd /help**
6. You'll notice that it makes use of mandatory and optional command-line arguments
7. It will also list several usage examples which can be adapted for personal use.

### Command-Line Arguments and their Purpose
#### Mandatory

##### **/binds**
This argument contain the path to the Elite Dangerous .binds file to sycnhronise

##### **/vap**
This argument contain the path to the VoiceAttack profile .vap file to sycnhronise

##### **/sync** 
This argument determines which file(s) get updated and has the following options:

+ **oneway\__to__vap** will analyse both files and then update the Voice Attack Profile file only so that any of its internal commands that reference a key binding is updated to match that of your Elite Dangerous .binds

+ **oneway\__to__binds** will analyse both files and then update any spare binding slots in your Elite Dangerous .binds

+ **twoway** will perform both of the above.

#### Optional
##### **/backup** 
Directory path for backup file(s). The directory will be created if required. Affected file(s) are backed up with a sequential number so that a full history of changes can be kept.

##### **/analysis** 
Directory path for generated operational analysis file(s). This can provide some interesting insight to such things, what bindings are actually available or what voice commands map to what binding. Use in conjunction with /format. If /format not used, csv is default.

##### **/format** 
File format for operational analysis file(s) (**csv** _default_, **htm**)

##### **/tag** 
Create reference tag in affected file(s). The tag is based on the current time stamp, has a format of EdVard[nnnnnnn] and is written into all affected file(s). This helps to identify updated profiles in particular when selecting from within Elite Dangerous.

##### The Action Dictionary
EDVard contains an internal dictionary that maps VoiceAttack Commands to Elite Dangerous bindings. 
e.g. **((50%))** in VoiceAttack maps to **Set_50_Percent** in Elite Dangerous.
For the most part, this doesn't require any changing, but you are given the option to override this dictionary with your own mappings should you wish to.

######  **/export** 
File path to export action dictionary

###### **/import** 
File path to import action dictionary

## Example
**EDVArd.exe** **/binds** "C:\Elite Dangerous\My.binds" **/vap** C:\HCSVoicePack\My.vap **/sync**:twoway **/analysis** desktop **/tag**

Example Description: The My.binds and My.vap files are scanned and the key bindings within My.vap synchronised to the existing key bindings within My.binds. As the synchronisation is: **twoway**, My.binds is also scanned for any vacant command slots where any additional commands within My.vap exist. If any are found, then My.binds is also updated. Operational Files will be saved to the user desktop in .csv format. Affected file(s) will be internally tagged.

#Final 
Once the synchronisation operation is complete, the following actions are required:

1. If an update to the Voice Attack Profile has occurred then for it to take effect, it has to be imported into Voice Attack and be made the current profile

2. If an update to the Elite Dangerous .binds file has occurred, then the updated .binds file has to reside in its usual %APPDATA% location. Ensure that it is re-selected in Options/Controls

3. If you haven't done so, train the **Windows Speech Recognition** engine to recognise your voice (Control Panel + Speech Recognition). Adjust microphone levels to minimise any distortion.

# Future Plans
## Front-end
Write a WPF-based GUI for those people uncomfortable with command-line utilities.