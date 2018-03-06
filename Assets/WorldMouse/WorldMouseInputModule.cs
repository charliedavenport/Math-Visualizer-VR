using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WorldMouseInputModule : BaseInputModule
{
	public WorldMouse[] worldMice;
	private Camera worldMouseCam;
	private PointerEventData[] eventData;
	private GameObject[] lastPressed;
				

	protected override void Start()
	{
		base.Start(); //required...baseinputmodule does something clearly :-)

		//we create a camera that will be our raycasting camera and add it to all canvases
		//this is necessary because unity requires a screen space position to do raycasts against
		//gui objects, and uses an "event camera" to do the actual raycasts
		worldMouseCam = new GameObject("Controller UI Camera").AddComponent<Camera>();
		worldMouseCam.nearClipPlane = .01f;  
		worldMouseCam.clearFlags = CameraClearFlags.Nothing; //note the camera renders nothing
		worldMouseCam.cullingMask = 0; //and no objects even try to draw to the camera
		Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
		foreach (Canvas canvas in canvases)
		{
			canvas.worldCamera = worldMouseCam;
		}

		eventData = new PointerEventData[worldMice.Length];
		for(int i = 0; i < eventData.Length; i++)
		{
			eventData[i] = new PointerEventData(base.eventSystem);
		}
		lastPressed = new GameObject[worldMice.Length];

	}

	// Process is called by UI system to process events.  
	public override void Process()
	{

		//the currently selected object may want to update something each frame
		BaseEventData data = GetBaseEventData();
		ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler); 

		//we process each world mouse separately, which encapsulates the concept of a mouse within a camera space that will raycast into the world
		for(int i = 0; i < eventData.Length; i++) 
		{
			WorldMouse wm = worldMice[i];
			
			wm.rayDistance = 0.0f;//assume nothing was hit to start
			
			//this makes the event system camera align with the world mouse
			worldMouseCam.transform.position = wm.transform.position;
			worldMouseCam.transform.forward = wm.transform.forward;
			//we reset the event data for the current frame.  Since the cursor doesn't actuall move, these are constant
			//this may cause some problems down the road...in which case we would probably create a camera for each canvas
			//and then move the cursor to the raycast intersection with the quad encapsulating the GUI, updating these deltas
			PointerEventData currentEventData = eventData[i];
			currentEventData.Reset();
			currentEventData.delta = Vector2.zero;
			currentEventData.position = new Vector2(worldMouseCam.pixelWidth / 2.0f, worldMouseCam.pixelHeight / 2.0f);
			currentEventData.scrollDelta = Vector2.zero;

			//this is where all the magic actually happens
			//the event system takes care of raycasting from all cameras at all cursor locations into the world
			base.eventSystem.RaycastAll(currentEventData, m_RaycastResultCache);
			currentEventData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
			if (wm.worldRayDistance < currentEventData.pointerCurrentRaycast.distance)
			{
				continue; //don't process anything if a world object was hit
			}
			//if we hit something this will not be null
			if (currentEventData.pointerCurrentRaycast.gameObject != null)
			{
				//this is useful to know where the object was hit (to draw a point or limit the lenght of a laser)
				wm.rayDistance = currentEventData.pointerCurrentRaycast.distance;
				//we can think of the object we hit as what we are hovering above (the simplest type)
				GameObject hoverObject = currentEventData.pointerCurrentRaycast.gameObject;
				
				// handle enter and exit events (highlight)
				base.HandlePointerExitAndEnter(currentEventData, hoverObject);

				//if the user clicks, other events may need to be handled
				if (wm.pressDown())
				{
					//if we click, we want to clear the current selection 
					//and fire associated events
					if (base.eventSystem.currentSelectedGameObject)
					{
						base.eventSystem.SetSelectedGameObject(null);
					}

					//these are important for those handling a click event
					currentEventData.pressPosition = currentEventData.position;
					currentEventData.pointerPressRaycast = currentEventData.pointerCurrentRaycast;

					//execute both the pointer down handler 
					GameObject handledPointerDown = ExecuteEvents.ExecuteHierarchy(hoverObject, currentEventData, ExecuteEvents.pointerDownHandler);
					//execute the click handler, either on the hoverObject if nothing handled  pointerdown or on whatever handled it
					GameObject handledClick = ExecuteEvents.ExecuteHierarchy(handledPointerDown==null?hoverObject:handledPointerDown, currentEventData, ExecuteEvents.pointerClickHandler);
					//something handled the click or pressed, so save a reference to it, needed later
					GameObject newPressed = handledClick != null ? handledClick : handledPointerDown; 
					
					//we need to deal with a new selection if the press/click was handled
					if (newPressed != null)
					{
						currentEventData.pointerPress = newPressed;
						if (ExecuteEvents.GetEventHandler<ISelectHandler>(newPressed))
						{
							base.eventSystem.SetSelectedGameObject(newPressed);
						}

					}
					//execute a drag start event on the currently pressed object and save it
					ExecuteEvents.Execute(newPressed, currentEventData, ExecuteEvents.beginDragHandler);
					currentEventData.pointerDrag = newPressed;

					//we save what was currently pressed for when we release
					lastPressed[i] = newPressed == null ? hoverObject : newPressed;
				}

				//handle releasing the "click"
				if (wm.pressUp())
				{
					if (lastPressed[i] != null)
					{
						ExecuteEvents.Execute(lastPressed[i], currentEventData, ExecuteEvents.endDragHandler);
						ExecuteEvents.ExecuteHierarchy(lastPressed[i], currentEventData, ExecuteEvents.dropHandler);
						ExecuteEvents.Execute(lastPressed[i], currentEventData, ExecuteEvents.pointerUpHandler);
						currentEventData.pointerDrag = null;
						currentEventData.rawPointerPress = null;
						currentEventData.pointerPress = null;
						lastPressed[i] = null;
					}
				}

				// drag handling
				if (lastPressed[i] != null)
				{
					ExecuteEvents.Execute(lastPressed[i], currentEventData, ExecuteEvents.dragHandler);
				}
			}
			
			
			m_RaycastResultCache.Clear();
		}
	}
}