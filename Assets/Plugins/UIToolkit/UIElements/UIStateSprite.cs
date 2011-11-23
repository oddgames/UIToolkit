using UnityEngine;

public class UIStateSprite : UISprite
{
    private bool _rollOverState = true;
    private int _state = 0;
    private int maxState = 0;
    private UIUVRect[] _uvFrames;


    #region Constructors
    public static UIStateSprite create(string filename, int xPos, int yPos)
    {
        return UIStateSprite.create(UI.firstToolkit, filename, xPos, yPos);
    }

    public static UIStateSprite create(UIToolkit manager, string filename, int xPos, int yPos)
    {
        return UIStateSprite.create(manager, filename, xPos, yPos, 1);
    }

    public static UIStateSprite create(UIToolkit manager, string filename, int xPos, int yPos, int depth)
    {
        var filenames = new string[1] { filename };
        return UIStateSprite.create(manager, filenames, xPos, yPos, depth);
    }

    public static UIStateSprite create(string[] filenames, int xPos, int yPos)
    {
        return UIStateSprite.create(UI.firstToolkit, filenames, xPos, yPos);
    }

    public static UIStateSprite create(UIToolkit manager, string[] filenames, int xPos, int yPos)
    {
        return UIStateSprite.create(manager, filenames, xPos, yPos, 1);
    }

    public static UIStateSprite create(UIToolkit manager, string[] filenames, int xPos, int yPos, int depth)
    {
        // Grab the texture details for the initial state
        UITextureInfo firstNormalTI = manager.textureInfoForFilename(filenames[0]);
        Rect frame = new Rect(xPos, yPos, firstNormalTI.frame.width, firstNormalTI.frame.height);

        // create the button
        UIStateSprite sprite = new UIStateSprite(manager, frame, depth, firstNormalTI.uvRect);
        // Add frames
        sprite.addFrames(filenames);

        return sprite;
    }

    public UIStateSprite(UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame)
        : base(frame, depth, uvFrame)
    {
        manager.addSprite(this);
    }
    #endregion

    /// <summary>
    /// Flag whether state loops when stats exceeds limits
    /// </summary>
    public bool rollOverState
    {
        get { return _rollOverState; }
        set { _rollOverState = value; }
    }

    public int state
    {
        get { return _state; }
        set
        {
            if (_state == value) return;

            _state = value;
            adjustForStateRollover(_state);
            setFrameForState(_state);
        }
    }

    public UIUVRect[] uvFrames
    {
        get { return _uvFrames; }
    }

    public void addFrames(string[] normal)
    {
        addFrames(loadFrames(normal));
    }

    public void addFrames(UIUVRect[] normal)
    {
        _uvFrames = normal;
        maxState = _uvFrames.Length;
        _state = 0;
    }

    private void adjustForStateRollover(int newState)
    {
        if (_state >= maxState)
        {
            if (_rollOverState)
            {
                _state = 0;
            }
            else
            {
                _state = maxState - 1;
            }
        }
        else if (_state < 0)
        {
            if (_rollOverState)
            {
                _state = maxState - 1;
            }
            else
            {
                _state = 0;
            }
        }
    }

    private void setFrameForState(int newState)
    {
        base.uvFrame = _uvFrames[newState];
    }

    private UIUVRect[] loadFrames(string[] filenames)
    {
        UIUVRect[] frames = new UIUVRect[filenames.Length];
        for (int i = 0; i < filenames.Length; i++)
        {
            frames[i] = manager.textureInfoForFilename(filenames[i]).uvRect;
        }
        return frames;
    }
}
