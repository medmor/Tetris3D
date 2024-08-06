using UnityEngine;

public delegate void MoveAndRotate();
public class InputManager : Manager<InputManager>
{
    public Events.ControlsInput ControlsInput { get; private set; } = new Events.ControlsInput();
    public Events.SwipInput SwipInput { get; private set; } = new Events.SwipInput();

    public Camera SideCamera;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    GameObject hitObject = default;

    Enums.Directions swipDirection = default;
    private void Update()
    {
        if (GameManager.Instance.CurrentGameState == Enums.GameState.RUNNING)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                ControlsInput.Invoke(Enums.ControlsEvents.LEFT);

            else if (Input.GetKeyDown(KeyCode.RightArrow))
                ControlsInput.Invoke(Enums.ControlsEvents.RIGHT);

            else if (Input.GetKeyDown(KeyCode.DownArrow))
                ControlsInput.Invoke(Enums.ControlsEvents.RIGHTROTATION);

            else if (Input.GetKeyDown(KeyCode.UpArrow))
                ControlsInput.Invoke(Enums.ControlsEvents.LEFTROTATION);

            else if (Input.GetKeyDown(KeyCode.Space))
                ControlsInput.Invoke(Enums.ControlsEvents.DROP);

            else if (Input.GetKeyDown(KeyCode.P))
                GameManager.Instance.TogglePause();

            else if (Input.GetKeyDown(KeyCode.D))
                ControlsInput.Invoke(Enums.ControlsEvents.DOWN);

            Swipe();
        }
    }

    public void Swipe()
    {
        if (!SideCamera)
            SideCamera = GameObject.Find("Cameras/SideCamera").GetComponent<Camera>();
        if (Input.touches.Length > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                firstPressPos = new Vector2(touch.position.x, touch.position.y);

            var ray = SideCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hitObject == null)
            {
                if (hit.collider.gameObject.tag == "SideBrick")
                {
                    hitObject = hit.collider.gameObject;
                }
            }
            else
            {
                ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hit))
                    if (hit.collider.gameObject.tag == "Grid")
                        hitObject = hit.collider.gameObject;
            }

            if (touch.phase == TouchPhase.Ended && hitObject != null)
            {
                secondPressPos = new Vector2(touch.position.x, touch.position.y);

                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                if (Vector2.SqrMagnitude(currentSwipe) > 500)
                {
                    currentSwipe.Normalize();
                    if (Mathf.Abs(currentSwipe.x) > Mathf.Abs(currentSwipe.y))
                    {
                        if (currentSwipe.x < 0)
                            swipDirection = Enums.Directions.LEFT;

                        else
                            swipDirection = Enums.Directions.RIGHT;
                    }
                    else
                        if (currentSwipe.y < 0)
                        swipDirection = Enums.Directions.DOWN;
                    else
                        swipDirection = Enums.Directions.TOP;

                    int temp = -1;
                    if (hitObject.GetComponent<Cube>())
                        temp = hitObject.GetComponent<Cube>().Id;
                    hitObject = null;
                    SwipInput.Invoke(swipDirection, temp);
                }
            }
        }
    }
}
