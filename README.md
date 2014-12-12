
&nbsp;Unity Input Manager pain is lasting for years. On Unity site you can find request for InputManager programmatic access dating from 2004, and some new from 2014. Futile.</div>
<div style="text-align: justify;">
<br />
Problems:<br />
1) Ingame input controller mapping.<br />
&nbsp; &nbsp;Unity offer user interface so you can map input before start the game. Changing that setting need game restart.<br />
2) Handling input in code based on states/tag abstracting real input.<br />
Some sort of abstraction is done thru InputManager well known "Horizontal" and "Vertical", but that abstraction is still bonded to axes and buttons and not to actions/FSM of the game(one of those Mecanima).<br />
<br />
3) Saving and restoring user preferences<br />
<a href="https://www.google.com/search?client=safari&amp;rls=en&amp;q=PlayerPref&amp;ie=UTF-8&amp;oe=UTF-8">PlayerPref</a>&nbsp;might do the trick<span style="font-family: inherit;">&nbsp;if you not plan to support Web, Droid and if&nbsp;<span style="line-height: 21px; text-align: start;">file size is not bigger then 1 megabyte.</span></span><br />
<br />
4) Recognize positive and negative part of the axis.<br />
Unity recognize Axis as whole part and gives value 1 &lt;--- (-1) ---&gt; 1. &nbsp;Its expected to gives value from 1&lt;---- (0) ----&gt; -1, so recognition of for example turning left/right on wheel input controller or joystick push/pull &nbsp;forward/backward is possible, trigger &nbsp;shooting rapid fire... .<br />
<br />
5) OS independent driver and easy expansion with drivers supporting other devices and special properties Unity internal handler might not recognize device or identify same button in different system differently and not offer support of device extra features like force feedback, IR pointer, accelerators, gyros ...of modern input controllers. So instead of offer of pluggin-paid OS dependent drivers, seem much better OS dependent HID interfaces with OS independent pluggable drivers.<br />
<br />
6) Handling input axis and buttons as digital or analog<br />
In Unity thru Input class you can handle axes only as analog and buttons as analog/digital. Its handy to have axes as digital events like in fighting game scenario 2times joystick left + fire (Mustafa kick<br />
<span dir="auto">Cadillacs and Dinosaurs)</span><br />
<br />
<br />
7) Create combination of inputs that would trigger action/state<br />
Unity doesn't offer out of the box combining input action like 2 keys in row ...don't want to speak of combination of axis and keys and mouse together.<br />
<br />
8) Handling inputs by events<br />
Seem whole Unity engine is not much planned as event-based, signal or reaction based system and encouraging of use of Broadcast messaging and handling complex input inside Update is far from good solution even if you need last processor ns.<br />
<br />
9) Plug and play instead of plug and pray.<br />
Attach or remove controllers while game is running.<br />
<br />
10) Profiles - Layouts<br />
<br />
<span style="color: red;">Why not InControl or CInput?&nbsp;</span><br />
Both are based on bad foundation which is Unity InputManager and even they give you runtime mapping and change they have same sickness even worse.<br />
They create InputManager.asset with all possible Joystick#+Buttons# (Joystick1Button1,...Joystick2Button2... combinations and by reading name of joystick they give you button layout(profiles) calming they support bunch of devices. Actually they didn't support anything else then what Unity default driver support(on button-axis level),... so NO AXIS DIR RECOGNITION, NO PLUG and PLAY, NO FFD or SPECIAL FEATURES ACCELEROMETERS, GYROS, IR CAM...., NO SUPPORT EXPANSION TO NEW DEVICES...AND NOT FREE<br />
<br />
<br /></div>
<div style="text-align: justify;">
Input Mapper system try to address above issues.<br />
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
<object class="BLOGGER-youtube-video" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0" data-thumbnail-src="https://i1.ytimg.com/vi/kX0j69ZgeCw/0.jpg" height="266" width="320"><param name="movie" value="https://www.youtube.com/v/kX0j69ZgeCw?version=3&f=user_uploads&c=google-webdrive-0&app=youtube_gdata" /><param name="bgcolor" value="#FFFFFF" /><param name="allowFullScreen" value="true" /><embed width="320" height="266"  src="https://www.youtube.com/v/kX0j69ZgeCw?version=3&f=user_uploads&c=google-webdrive-0&app=youtube_gdata" type="application/x-shockwave-flash" allowfullscreen="true"></embed></object></div>
<div style="text-align: justify;">
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<br />
<br />
<br />
<b>1) Ingame input controller mapping.</b><br />
Input Mapper allows you easily to map Animation States from your Animation Controller or custom states<br />
<br />
<b>7) Create combination of inputs that would trigger action/state</b></div>
<div style="text-align: justify;">
Map inputs to state by just clicking keys/mouse/buttons or moving joystick as SINGLE, DOUBLE and LONG and make combination of inputs(Combos). Primary and secondary. In bellow example I've map to Wave state combo of Mouse1 double clicks + Joystick1AxisYForward (Long push forward of the Joystick) + double click of letter Y.</div>
<br />
<div class="separator" style="clear: both; text-align: center;">
<a href="http://4.bp.blogspot.com/-QozKhWjL7CE/U7Fi8W5R-7I/AAAAAAAAATU/xM2xN0pe0mw/s1600/Clipboard01.jpg" imageanchor="1" style="margin-left: 1em; margin-right: 1em;"><img border="0" src="http://4.bp.blogspot.com/-QozKhWjL7CE/U7Fi8W5R-7I/AAAAAAAAATU/xM2xN0pe0mw/s1600/Clipboard01.jpg" /></a></div>
<div class="" style="clear: both; text-align: center;">
You can set modifier format or clicks sensitivity.&nbsp;</div>
<div class="" style="clear: both; text-align: center;">
<br /></div>
<div class="" style="clear: both; text-align: left;">
<span style="text-align: justify;"><b>3) Saving and restoring user preferences</b></span></div>
<div class="" style="clear: both; text-align: center;">
<div style="text-align: left;">
Saving would export your settings to .xml file and States.cs will be generated containing enum of your mapped states.</div>
<div style="text-align: left;">
<br /></div>
</div>
<pre class="brush:csharp;">public enum States:int{
  Wave=1397315813,

         MyCustomState=-1624475888,
</pre>
<pre class="brush:csharp;"></pre>
Now you can forget about:<br />
<pre class="brush:csharp;">//  static int idleState = Animator.StringToHash("Base Layer.Idle"); 
//  static int locoState = Animator.StringToHash("Base Layer.Locomotion");  
</pre>
as we are tought in Unity tutorials, but you can use States.[some state].<br />
<br />
Library contains simple component so you can test user perspective right away. Just drag saved .xml and Play.<br />
<div class="separator" style="clear: both; text-align: center;">
<a href="http://2.bp.blogspot.com/-uDEc2GppzQU/U7FoSxJCOlI/AAAAAAAAATk/ObMimVnqEkk/s1600/Clipboard02.jpg" imageanchor="1" style="margin-left: 1em; margin-right: 1em;"><img border="0" src="http://2.bp.blogspot.com/-uDEc2GppzQU/U7FoSxJCOlI/AAAAAAAAATk/ObMimVnqEkk/s1600/Clipboard02.jpg" height="113" width="320" /></a></div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: left;">
<span style="text-align: justify;"><b>2) Handling input in code based on states/tag abstracting real input.</b></span></div>
<div class="" style="clear: both; text-align: justify;">
InputMapper API are very similar to Unity API's with the big difference and that is abstraction on 2 levels.First you not program with real inputs like KeyCode.Q or Joystick1Button99, but with states which also allows player to map different input to same action state.</div>
<pre class="brush:csharp;">if(InputManager.GetInputDown((int)States.Wave)){
  Debug.Log("Wave Down");
   }
</pre>
<pre class="brush:csharp;"></pre>
<pre class="brush:csharp;"><span style="font-family: Times; text-align: justify; white-space: normal;"><b>6) Handling input axis and buttons as digital or analog</b></span></pre>
Second abstraction is that you can use digital or analog output no matter your real input source is digital or analog meaning for ex. Joystick Axis count as analog source can produce normalized values from 0 to 1, but also pushed true/false, or key or even mouse button.
<br />
<pre class="brush:csharp;">//using input as digital
bool bHold=(InputManager.GetInput((int)States.Walk_Forward,false));

//using input as analog value   
float analogValue=InputManager.GetInput((int)States.Walk_Forward,false,0.3f,0.1f,0f);
  
</pre>
<span style="font-family: Times; text-align: justify; white-space: normal;"><b>8) Handling inputs by events</b></span>
<br />
<div class="" style="clear: both; text-align: left;">
As Update method get overcrowded, library offer modern input handling solution by use of Event based system.</div>
<pre class="brush:csharp;">  //Event Based input handling
            InputEvent ev = new InputEvent("Click_W+C_State");
    //InputEvent ev = new InputEvent((int)States.SomeState);
        
            ev.CONT+=new EventHandler(Handle1);
            ev.CONT+= new EventHandler(Handle2);
            ev.UP += new EventHandler(onUp);//this wouldn't fire for combo inputs(single only)
            ev.DOWN += new EventHandler(onDown);//this wouldn't fire for combo inputs(single only)
 }


    void onUp(object o, EventArgs args)
    {
        Debug.Log("Up");
    }

    void onDown(object o, EventArgs args)
    {
        Debug.Log("Down");
    }

    void Handle1(object o, EventArgs args)
    {
        Debug.Log("Handle1");
    }

    void Handle2(object o, EventArgs args)
    {
        Debug.Log("Handle2");
    }
</pre>
<div class="" style="clear: both; text-align: center;">
Hardcode developers can manually map input to states and even mix with loaded settings.</div>
<pre class="brush:csharp;"> InputManager.loadSettings(Path.Combine(Application.streamingAssetsPath,"InputSettings.xml"));

        
       
   //  adding input-states pairs manually
   InputManager.MapStateToInput("My State1",new InputCombination("Mouse1+Joystick12AxisXPositive(x2)+B"));

   InputManager.MapStateToInput("Click_W+C_State", KeyCodeExtension.Alpha0.DOUBLE,KeyCodeExtension.JoystickAxisPovYPositive.SINGLE);

</pre>
<div class="" style="clear: both; text-align: center;">
In KeyCodeExtension all KeyCode stuff are supported plus additional for Joystick Axis mapping.</div>
<div class="" style="clear: both; text-align: center;">
<br /></div>
<div class="" style="clear: both; text-align: center;">
<br /></div>
<div class="" style="clear: both; text-align: left;">
<span style="text-align: justify;"><b>5) OS independent driver and easy expansion with drivers supporting other devices and special properties</b></span></div>
<div class="" style="clear: both; text-align: center;">
<div style="text-align: justify;">
I understood Unity couldn't support all game devices but some system that allows simple binding of driver might be good idea (Yeah they have pluggins.....). &nbsp;</div>
</div>
<div class="" style="clear: both; text-align: center;">
So instead building plugging for all OSes InputMapper system had built HID interface systems for (Win,Web,Droid and OS) that allows writing only one device specified driver OS in independent.</div>
<div class="" style="clear: both; text-align: center;">
<br /></div>
<div class="" style="clear: both; text-align: center;">
Your implementation of custom device driver had 2 entry points that need implementation:</div>
<pre class="brush:csharp;"> 
 (1)public IJoystickDevice ResolveDevice(IHIDDeviceInfo info)...

 (2)public void Update(IJoystickDevice joystick)...
</pre>
<div style="text-align: justify;">
Resolve device where HIDInterface provide device info from the OS, you can check VID and PID to decide to handle and init device properties and structures, or not returning null,</div>
<div style="text-align: justify;">
and Update function to query device by use of provided <a href="https://github.com/winalex/Unity3d-InputMapper/blob/master/Assets/Scripts/ws/winx/drivers/XInputDriver.cs">Read/Write methods</a> and fill the JoystickDevice structures, so can be accessed by InputManager.</div>
<br />
Device would be handled by default driver (WinMMDriver for Win and OSXDriver for OSX) or by custom driver if added like:<br />
<pre class="brush:csharp;"> 
//supporting devices with custom drivers
   InputManager.AddDriver(new XInputDriver());
</pre>
<br />
Devices used during testing: XBox360W controller, ThrustMaster Wheel FFD, Wiimote + Nunchuk.<br />
One classic Gamepad controller, one Wheel and complex controller.<br />
<br />
Still want to use Unity InputManger as backup
<br />
<pre class="brush:csharp;"> InputManager.AddDriver(new UnityDriver());//(-1 to 1 to -1)
</pre>
Swap your InputManger.asset with InputManager.asset from github source code.
<br />
<div style="text-align: center;">
<div class="" style="clear: both; text-align: center;">
'</div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: left;">
19.08.14 </div>
<a href="https://www.youtube.com/watch?v=uoH-RfopGzk&feature=youtu.be" >Thrustmaster wheel RGT FFD demo WIN+DROID </a>
<br />
<div class="separator" style="clear: both; text-align: center;">
<object class="BLOGGER-youtube-video" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0" data-thumbnail-src="https://i.ytimg.com/vi/uoH-RfopGzk/0.jpg" height="266" width="320"><param name="movie" value="https://www.youtube.com/v/uoH-RfopGzk?version=3&f=user_uploads&c=google-webdrive-0&app=youtube_gdata" /><param name="bgcolor" value="#FFFFFF" /><param name="allowFullScreen" value="true" /><embed width="320" height="266"  src="https://www.youtube.com/v/uoH-RfopGzk?version=3&f=user_uploads&c=google-webdrive-0&app=youtube_gdata" type="application/x-shockwave-flash" allowfullscreen="true"></embed></object></div>
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
GITHUB:&nbsp;<a href="https://github.com/winalex/Unity3d-InputMapper">https://github.com/winalex/Unity3d-InputMapper</a></div>
<div class="separator" style="clear: both; text-align: left;">
Code is free if you contribute by solving bug or enhance something !!! Joking :) Who can stop you/us.</div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
Feedback, Forks are welcome if you aren't too smart, too movie star or too busy making money.</div>
<div class="separator" style="clear: both; text-align: center;">
Gmail me as winxalex.</div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
Knowledge should be free.&nbsp;</div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: left;">
13.07.2014 Added WiimoteDevice and WiimoteDriver Skeleton</div>
<div class="separator" style="clear: both; text-align: left;">
22.07.2014 Quite more stability and plug in/out supported added.</div>
<div class="separator" style="clear: both; text-align: left;">
26.07.2014 <a href="http://unity3de.blogspot.com/2014/07/unity3d-inputmapper-joystick-in-web.html">Web Player joystick support </a>(Chrome, FireFox)</div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div style="text-align: center;">
<div class="separator" style="clear: both; text-align: center;">
<object class="BLOGGER-youtube-video" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0" data-thumbnail-src="https://i.ytimg.com/vi/tEwMcA2ZaMk/0.jpg" height="266" width="320"><param name="movie" value="https://www.youtube.com/v/tEwMcA2ZaMk?version=3&f=user_uploads&c=google-webdrive-0&app=youtube_gdata" /><param name="bgcolor" value="#FFFFFF" /><param name="allowFullScreen" value="true" /><embed width="320" height="266"  src="https://www.youtube.com/v/tEwMcA2ZaMk?version=3&f=user_uploads&c=google-webdrive-0&app=youtube_gdata" type="application/x-shockwave-flash" allowfullscreen="true"></embed></object></div>
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: left;">
01.10.2014 WiiDevice and WiiDriver&nbsp;</div>
<div class="separator" style="clear: both; text-align: left;">
05.10.2014 XInput driver pure C# (No DirectX xinput.dll wrappers)</div>
<div class="separator" style="clear: both; text-align: left;">
13.10.2014 (OSXDriver default driver pure C#)</div>
<div class="separator" style="clear: both; text-align: left;">
17.10.2014 <a href="https://www.youtube.com/watch?v=RpjxBU31k0o">(Thrustmaster Wheel FFD and XBOX360W working on OSX)</a>&nbsp;</div>
<div class="separator" style="clear: both; text-align: left;">
<b>CONCEPT PROVED!!!</b></div>
<div class="separator" style="clear: both; text-align: left;">
<br /></div>
<div class="separator" style="clear: both; text-align: left;">
<br /></div>
<div style="text-align: center;">
<object class="BLOGGER-youtube-video" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0" data-thumbnail-src="https://i.ytimg.com/vi/RpjxBU31k0o/0.jpg" height="266" width="320"><param name="movie" value="https://www.youtube.com/v/RpjxBU31k0o?version=3&f=user_uploads&c=google-webdrive-0&app=youtube_gdata" /><param name="bgcolor" value="#FFFFFF" /><param name="allowFullScreen" value="true" /><embed width="320" height="266"  src="https://www.youtube.com/v/RpjxBU31k0o?version=3&f=user_uploads&c=google-webdrive-0&app=youtube_gdata" type="application/x-shockwave-flash" allowfullscreen="true"></embed></object><br />
<br /></div>
<div class="separator" style="clear: both; text-align: left;">
<b>PROFILES:</b></div>
<div class="separator" style="clear: both; text-align: center;">
</div>
<div style="-webkit-text-stroke-width: 0px; color: black; font-family: Times; font-size: medium; font-style: normal; font-variant: normal; font-weight: normal; letter-spacing: normal; line-height: normal; orphans: auto; text-align: justify; text-indent: 0px; text-transform: none; white-space: normal; widows: auto; word-spacing: 0px;">
</div>
<br />
<div style="-webkit-text-stroke-width: 0px; color: black; font-family: Times; font-size: medium; font-style: normal; font-variant: normal; font-weight: normal; letter-spacing: normal; line-height: normal; orphans: auto; text-align: justify; text-indent: 0px; text-transform: none; white-space: normal; widows: auto; word-spacing: 0px;">
<div style="margin: 0px;">
I'll show you some of next post how you show for example WiiButtonA instead JoystickButton1....</div>
</div>
<br />
<script type="text/javascript">
 SyntaxHighlighter.highlight();
</script>
