var freeCam: any = null;
var normalspeed = 0.5;
var highspeed = 1.5;

API.onServerEventTrigger.connect((eventName, args) => {
    if(eventName == "FreeCamStart")
    {
        freeCam = API.createCamera(args[0], new Vector3(0, 0, 0));
        API.setActiveCamera(freeCam);
    }
    if(eventName == "FreeCamStop")
    {
        var cameraPos = API.getCameraPosition(freeCam);
        API.triggerServerEvent("ChangePosition", new Vector3(cameraPos.X, cameraPos.Y, cameraPos.Z + 1));
        API.setActiveCamera(null);
        freeCam = null;
    }
});

API.onUpdate.connect(() =>
{
    if(freeCam != null)
    {
        API.setCameraRotation(freeCam, API.getGameplayCamRot());	
        var cameraRot = API.getCameraRotation(freeCam);
        var cameraPos = API.getCameraPosition(freeCam);
        API.setEntityPosition(API.getLocalPlayer(), new Vector3(cameraPos.X, cameraPos.Y, cameraPos.Z-2.5));
        API.setEntityRotation(API.getLocalPlayer(), new Vector3(0, 0, cameraRot.Z));
        var pi = Math.PI;
        var xradian = ((cameraRot.X*pi) / 180);
        var yradian = ((cameraRot.Z*pi) / 180);
        var zradian = ((cameraRot.Z*pi) / 180);
        if(API.isControlPressed(87)) // W button normal speed move straight
        {
            API.enableControlThisFrame(87);			
            var OldPos = API.getCameraPosition(freeCam);
            var newx = -(Math.sin(yradian)*normalspeed);
            var newy = Math.cos(yradian)*normalspeed; 
            var newz = Math.sin(xradian)*normalspeed; // up or down					
            API.setCameraPosition(freeCam, new Vector3(OldPos.X+newx, OldPos.Y+newy, OldPos.Z+newz));	

        }
        if(!API.isChatOpen())
        {
            if(API.isControlPressed(21)) // Left Shift button high speed move 
            {	
                if(API.isControlPressed(87)) // straight
                {
                    API.enableControlThisFrame(21);	
                    var OldPos = API.getCameraPosition(freeCam);
                    var newx = -(Math.sin(yradian)*highspeed);
                    var newy = Math.cos(yradian)*highspeed; 
                    var newz = Math.sin(xradian)*highspeed;						
                    API.setCameraPosition(freeCam, new Vector3(OldPos.X+newx, OldPos.Y+newy, OldPos.Z+newz));				
                }
                if(API.isControlPressed(268)) // back
                {
                    API.enableControlThisFrame(87);			
                    var OldPos = API.getCameraPosition(freeCam);
                    var newx = Math.sin(yradian)*highspeed;
                    var newy = -(Math.cos(yradian)*highspeed); 
                    var newz = -(Math.sin(xradian)*highspeed); // up or down					
                    API.setCameraPosition(freeCam, new Vector3(OldPos.X+newx, OldPos.Y+newy, OldPos.Z+newz));	

                }
                if(API.isControlPressed(35)) // Right
                {
                    API.enableControlThisFrame(35);			
                    var OldPos = API.getCameraPosition(freeCam);
                    var newx = Math.cos(yradian)*highspeed;
                    var newy = Math.sin(yradian)*highspeed; 
                    var newz = -(Math.sin(xradian)*highspeed); // up or down					
                    API.setCameraPosition(freeCam, new Vector3(OldPos.X+newx, OldPos.Y+newy, OldPos.Z+newz));	

                }
                if(API.isControlPressed(34)) // Left
                {
                    API.enableControlThisFrame(34);			
                    var OldPos = API.getCameraPosition(freeCam);
                    var newx = -(Math.cos(yradian)*highspeed);
                    var newy = -(Math.sin(yradian)*highspeed); 
                    var newz = Math.sin(xradian)*highspeed; // up or down					
                    API.setCameraPosition(freeCam, new Vector3(OldPos.X+newx, OldPos.Y+newy, OldPos.Z+newz));	

                }
            }
        }
        if(API.isControlPressed(268)) // S button normal speed move back
        {
            API.enableControlThisFrame(87);			
            var OldPos = API.getCameraPosition(freeCam);
            var newx = Math.sin(yradian)*normalspeed;
            var newy = -(Math.cos(yradian)*normalspeed); 
            var newz = -(Math.sin(xradian)*normalspeed); // up or down					
            API.setCameraPosition(freeCam, new Vector3(OldPos.X+newx, OldPos.Y+newy, OldPos.Z+newz));	

        }
        if(API.isControlPressed(35)) // D button normal speed move right
        {
            API.enableControlThisFrame(35);			
            var OldPos = API.getCameraPosition(freeCam);
            var newx = Math.cos(yradian)*normalspeed;
            var newy = Math.sin(yradian)*normalspeed; 
            var newz = -(Math.sin(xradian)*normalspeed); // up or down					
            API.setCameraPosition(freeCam, new Vector3(OldPos.X+newx, OldPos.Y+newy, OldPos.Z+newz));	
        }
        if(API.isControlPressed(34)) // A button normal speed move left
        {
            API.enableControlThisFrame(34);			
            var OldPos = API.getCameraPosition(freeCam);
            var newx = -(Math.cos(yradian)*normalspeed);
            var newy = -(Math.sin(yradian)*normalspeed); 
            var newz = Math.sin(xradian)*normalspeed; // up or down					
            API.setCameraPosition(freeCam, new Vector3(OldPos.X+newx, OldPos.Y+newy, OldPos.Z+newz));	
        }
    }	
});