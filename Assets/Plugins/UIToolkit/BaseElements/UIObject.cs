using UnityEngine;
using System.Collections;


public class UIObject : System.Object, IPositionable
{
	public delegate void UIObjectTransormChagedDelegate();
	public event UIObjectTransormChagedDelegate onTransformChanged;
	
	private GameObject _client; // Reference to the client GameObject
    public GameObject client
    {
    	get { return _client; }
    }
    protected Transform clientTransform; // Cached Transform of the client GameObject
    private UIObject _parentUIObject;
    public virtual Color color { get; set; } // hack that is overridden in UISprite just for animation support
    
    
	/// <summary>
	/// Sets up the client GameObject along with it's layer and caches the transform
	/// </summary>
    public UIObject()
    {
		// Setup our GO
		_client = new GameObject( this.GetType().Name );
		_client.transform.parent = UI.instance.transform; // Just for orginization in the hierarchy
		_client.layer = UI.instance.layer; // Set the proper layer so we only render on the UI camera
		
		// Cache the clientTransform
		clientTransform = _client.transform;
    }

	
	#region Transform passthrough properties so we can update necessary verts when changes occur
	
	public virtual float zIndex
	{
		get { return clientTransform.position.z; }
		set
		{
			var pos = clientTransform.position;
			pos.z = value;
			clientTransform.position = pos;
		}
	}

	
	public virtual Vector3 position
	{
		get { return clientTransform.position; }
		set
		{
			clientTransform.position = value;
			if( onTransformChanged != null )
				onTransformChanged();
		}
	}


	public virtual Vector3 localPosition
	{
		get { return clientTransform.localPosition; }
		set
		{
			clientTransform.localPosition = value;
			if( onTransformChanged != null )
				onTransformChanged();
		}
	}
	
	
	public virtual Vector3 eulerAngles
	{
		get { return clientTransform.eulerAngles; }
		set
		{
			clientTransform.eulerAngles = value;
			if( onTransformChanged != null )
				onTransformChanged();
		}
	}


	public virtual Vector3 localScale
	{
		get { return clientTransform.localScale; }
		set
		{
			clientTransform.localScale = value;
			if( onTransformChanged != null )
				onTransformChanged();
		}
	}
	
	
	public virtual Transform parent
	{
		get { return clientTransform.parent; }
		set { clientTransform.parent = value; }
	}
	

	/// <summary>
	/// Setting the parentUIObject automatically sets up a listener for changes to the tranform.
	/// When the parent transform changes, UISprite's will automatically call updateTransform to keep their
	/// touch frames and actual positions in sync with the parent
	/// </summary>
	public UIObject parentUIObject
	{
		get { return _parentUIObject; }
		set
		{
			if( value == _parentUIObject )
				return;
			
			// remove the old listener if we have one
			if( _parentUIObject != null )
				_parentUIObject.onTransformChanged -= transformChanged;
			
			// reparent the UIObject in the same UIToolkit tree as it's children
			//if( value != null && value.parent != parent )
			//	value.parent = parent;
						
			_parentUIObject = value;
			
			// if we got a null value, then we are being removed from the UIObject so reparent to our manager
			if( _parentUIObject != null )
			{
				clientTransform.parent = _parentUIObject.clientTransform;
			}
			else
			{
				if( this.GetType() == typeof( UISprite ) )
					clientTransform.parent = ((UISprite)this).manager.transform;
				else
					clientTransform.parent = null;
			}
			
			// add the new listener
			_parentUIObject.onTransformChanged += transformChanged;
		}
	}
	
	#endregion
	
	
	public virtual void transformChanged()
	{
		
	}
	
	
	#region IPositionable implementation
	
	// subclasses should implement these methods if they want to take part in positioning!
	public float width
	{
		get { throw new System.NotImplementedException(); }
	}
	
	
	public float height
	{
		get { throw new System.NotImplementedException(); }
	}
	
	#endregion

}
