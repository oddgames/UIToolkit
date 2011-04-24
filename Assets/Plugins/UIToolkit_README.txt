Create an empty object at 0, 0, 0.  Move this object to a new layer, which in the example is simply called "UILayer".
Now add the GUISpriteUI script to this object.  Be sure to set the following options in the inspector:
- Max Sprite Count: the maximum total simultaneous sprites/GUI objects you will be using
- UILayer: the layer in which the UI will live

Next you will need to go to the main camera and remove the UILayer (in this example "UILayer") from the Culling Mask so that your GUI doesn't get rendered twice.

You will need the object to have a texture associated with it that uses the included "Particles/Alpha Blended Z-enabled" shader.

Lastly, create a script and in the Start method create your GUI.  Do not try to create your GUI in Awake as the GUISpriteUI may not be done initializing yet.