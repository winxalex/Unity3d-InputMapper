<div style="text-align: justify;">
&nbsp;Unity Input Manger pain is lasting for years. On Unity site you can find request for</div>
<div style="text-align: justify;">
InputManager programmatic access dating from 2004,and some new from 2014. Futile.</div>
<div style="text-align: justify;">
Flows of current InputManager are numerous and need to restart game just to change mapping as genial solution is one of them.I know there are libraries like cInput... based on trick modifing InputManager.asset with all possible? joystick,buttons,axes ... but wasn't satisfied of level of automation, and I beleave that level should be high so game developing is fun.<br />
<br /></div>
<div style="text-align: justify;">
Following that direction I started and build the InputMapper editor and library.<br />
<br /></div>
<div style="text-align: justify;">
Input Mapper allows you easily to map Animation States from your Animation Controller or custom states</div>
<div style="text-align: justify;">
to inputs by just clicking keys/mouse/buttons or moving joystick as SINGLE,DOUBLE and LONG and</div>
<div style="text-align: justify;">
&nbsp;make combination of inputs(Combos). Primary and secondary. In bellow example I've map to Wave state combo of Mouse1 double clicks + Joystick1AxisYForward (Long push forward of the Joystick) + double click of letter Y.</div>
<br />
<div class="separator" style="clear: both; text-align: center;">
<a href="http://4.bp.blogspot.com/-QozKhWjL7CE/U7Fi8W5R-7I/AAAAAAAAATU/xM2xN0pe0mw/s1600/Clipboard01.jpg" imageanchor="1" style="margin-left: 1em; margin-right: 1em;"><img border="0" src="http://4.bp.blogspot.com/-QozKhWjL7CE/U7Fi8W5R-7I/AAAAAAAAATU/xM2xN0pe0mw/s1600/Clipboard01.jpg" /></a></div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
You can set modifier format or clicks sensitivity. Saving would export your settings to .xml file and States.cs will be generated containing enum of your mapped states.</div>
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
<div class="separator" style="clear: both; text-align: justify;">
InputMapper API are very similar to Unity API's with the big difference and that is abstraction on 2 levels.First you not program with real inputs like KeyCode.Q or Joystick1Button99, but with states which also allows player to map different input to same action state.</div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<pre class="brush:csharp;">if(InputManager.GetInputDown((int)States.Wave)){
  Debug.Log("Wave Down");
   }
</pre>
Second abstraction is that you can use digital or analog output no matter your real input source is digital or analog meaning for ex. Joystick Axis is count as analog can produce normalized values from 0 to 1 but also pushed true/false, or key or even mouse button.
<br />
<pre class="brush:csharp;">//using input as digital
bool bHold=(InputManager.GetInput((int)States.Walk_Forward,false));

//using input as analog value   
float analogValue=InputManager.GetInput((int)States.Walk_Forward,false,0.3f,0.1f,0f);
  
</pre>
<div class="separator" style="clear: both; text-align: left;">
As Update method get overcrowded library offer modern input handling solution by use of Event based system.</div>
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
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
Hardcode developers can manually map input to states and even mix with loaded settings.</div>
<pre class="brush:csharp;"> InputManager.loadSettings(Path.Combine(Application.streamingAssetsPath,"InputSettings.xml"));

        
       
   //  adding input-states pairs manually
   InputManager.MapStateToInput("My State1",new InputCombination("Mouse1+Joystick12AxisXPositive(x2)+B"));

   InputManager.MapStateToInput("Click_W+C_State", KeyCodeExtension.Alpha0.DOUBLE,KeyCodeExtension.JoystickAxisPovYPositive.SINGLE);

</pre>
<div class="separator" style="clear: both; text-align: center;">
In KeyCodeExtension all KeyCode stuff are supported plus additional for Joystick Axis mapping.</div>
<div class="separator" style="clear: both; text-align: center;">
I understood Unity couldn't support all game devices but some system that allows simple binding of driver might be good idea(Yeah they have pluggins.....). &nbsp;So I built small HID interface system that allows you to bind your driver for custom devices(at least similar to joystick/pad) with 2 points that your driver need to handle:</div>
<pre class="brush:csharp;"> 
 (1)public IJoystickDevice ResolveDevice(IHIDDeviceInfo info)...

 (2)public void Update(IJoystickDevice joystick)...
</pre>
Resolve device where HIDInterface provide you with device info from the OS, you can check VID and PID to decide to handle device or not returning null. Your Update function should query device and fill the structures so can be accessed by InputManager. If you have async handling you don't need to implement Update.At start add your device driver like this:
<br />
<pre class="brush:csharp;"> 
//supporting devices with custom drivers
   InputManager.AddDriver(new XInputDriver());
</pre>
Please check XInputDriver.cs(Someone with this kind of device pls test)  and WinMMDriver.cs (Default one that would try to handle device if custom driver isn't supplied).
There is similar HIDInterface for OSX as for WIN but didn't have chance to try it.(APPLE GUYS HELP!)
<br />
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div style="text-align: center;">
<object class="BLOGGER-youtube-video" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0" data-thumbnail-src="https://i1.ytimg.com/vi/9Y9JcD-rUb0/0.jpg" height="266" width="320"><param name="movie" value="https://www.youtube.com/v/9Y9JcD-rUb0?version=3&f=user_uploads&c=google-webdrive-0&app=youtube_gdata" /><param name="bgcolor" value="#FFFFFF" /><param name="allowFullScreen" value="true" /><embed width="320" height="266"  src="https://www.youtube.com/v/9Y9JcD-rUb0?version=3&f=user_uploads&c=google-webdrive-0&app=youtube_gdata" type="application/x-shockwave-flash" allowfullscreen="true"></embed></object></div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
GITHUB:&nbsp;<a href="https://github.com/winalex/Unity3d-InputMapper">https://github.com/winalex/Unity3d-InputMapper</a></div>
<div class="separator" style="clear: both; text-align: center;">
Code is free if you contribute by solving bug or ehance some version.!!!</div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
Feedback, Forks are welcome if you aren't too smart, too movie star or too busy making money.</div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
Further thoughts are on direction of making interface for drivers for not joystick like devices with easy KeyCodeExtension expansion. Knowledge should be free.</div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<div class="separator" style="clear: both; text-align: center;">
13.07.2014 Added WiimoteDevice and WiimoteDriver.</div>
<div class="separator" style="clear: both; text-align: center;">
<br /></div>
<br />
<script type="text/javascript">
 SyntaxHighlighter.highlight();
</script>
