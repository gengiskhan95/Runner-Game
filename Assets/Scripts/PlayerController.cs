using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Bools")]
	public bool isLevelStart;
	public bool isLevelDone;
	public bool isLevelFail;
	public bool isJumping;
	public bool isFalling;
	public bool isSwiping;
	public bool isClimbing;
	public bool isLanding;

	[Header("Speeds")]
	public float screenDwideCount;
	public float speedForward;
	public float smoothSpeed;
	public float speedSide;
	public float jumpSpeed;

	[Header("Limits")]
	public float groundLimitXL;
	public float groundLimitXB;
	public float groundLevel;

	[Header("Swipe Settings")]
	[Tooltip("Ýstenilen Kayma Hareketi Süresi")]
	public float SwipeTime;
	public float swipeMultiplier;
	public float ccCenterMinY;
	float swipeCounter;
	CapsuleCollider CC;
	float CapsuleColliderHeight;
	Vector3 ccCenter;
	Vector3 ccTempCenter;

	[Header("Climb & Falling Settings")]
	[Tooltip("Týrmanma Hýzý")]
	public float climbSpeed;
	[Tooltip("Düþme Hýzý")]
	public float fallingSpeed;
	[Tooltip("Düþerkenki Ýleriye Doðru Hýz")]
	public float fallingForwardSpeed;



	[Space(15)]
	#region Movement
	public float mouseFirstPosX;
	public float mouseFirstPosY;
	public float firstPosX;
	public float JumpHeight;
	public float JumpThreshold;
	public Vector3 targetPosition;
	Vector3 jumpPosition;
	Rigidbody RB;

	#endregion

	#region Animation
	[Header("Animator & Settings")]
	public Animator anim;

	public string animIsMoving = "isMoving";
	public string animIsJumping = "isJumping";
	public string animHorizontal = "horizontal";
	public string animIsMovingRight = "isMovingRight";
	public string animIsSwipe = "isSwiping";
	public string animIsDance = "isDance";
	public string animIsFall = "isFall";
	public string animCoke = "ColaAnim";
	public string animIsLanding = "isLanding";
	public string animIsClimbing = "isClimbing";
	#endregion

	#region Tags

	string TagObstacle;
	string TagFinish;
	string TagCollectable;
	string TagWallTrigger;

	#endregion
	public int coin;

	GameController GC;
	UIController UI;

	public static PlayerController instance;

	private void Awake()
	{
		if (!instance)
		{
			instance = this;
		}
	}
	// Start is called before the first frame update
	void Start()
	{
		StartMethods();
	}
	#region StartMethods

	void StartMethods()
	{
		GC = GameController.instance;
		UI = UIController.instance;

		GetSpeed();
		GetTags();
		GetRigidBody();
		//GetAnimator();
		GetCapsuleCollider();
	}

	void GetTags()
	{
		TagObstacle = GC.TagObstacle;
		TagCollectable = GC.TagCollectable;
		TagFinish = GC.TagFinish;
		TagWallTrigger = GC.TagWallTrigger;
	}

	void GetSpeed()
	{
		speedSide = Screen.width / screenDwideCount;
		JumpThreshold = Screen.height / screenDwideCount * 2;
	}
	void GetRigidBody()
	{
		RB = gameObject.GetComponent<Rigidbody>();
	}
	void GetAnimator()
    {
		anim = gameObject.GetComponent<Animator>();
	}

	void GetCapsuleCollider()
    {
		CC = gameObject.GetComponent<CapsuleCollider>();
		CapsuleColliderHeight = CC.height;
		ccCenter = CC.center;
    }
	#endregion

	#region TapToStartActions

	public void TapToStartActions()
	{
		AnimRun();
	}
	#endregion
	// Update is called once per frame
	void Update()
	{
		if (isLevelStart && !isLevelDone && !isLevelFail)
		{
			targetPosition = transform.position;


            if (!isClimbing && !isLanding)
            {
				targetPosition.z += speedForward;

				if (Input.GetMouseButtonDown(0))
				{
					mouseFirstPosX = Input.mousePosition.x;
					firstPosX = transform.position.x;
					mouseFirstPosY = Input.mousePosition.y;
				}
				else if (Input.GetMouseButton(0))
				{
					if (Input.mousePosition.x != mouseFirstPosX)
					{
						targetPosition.x = firstPosX + (Input.mousePosition.x - mouseFirstPosX) / speedSide;
						targetPosition.x = targetPosition.x > groundLimitXB ? groundLimitXB : targetPosition.x;
						targetPosition.x = targetPosition.x < groundLimitXL ? groundLimitXL : targetPosition.x;
						if (targetPosition.x > transform.position.x)
						{
							AnimTurnRight();
						}
						else if (targetPosition.x < transform.position.x)
						{
							AnimTurnLeft();
						}
						else
						{
							AnimDirect();
						}


						if (Input.mousePosition.y >= mouseFirstPosY + JumpThreshold && transform.position.y == groundLevel && !isSwiping)
						{
							isJumping = true;

							AnimJump();
						}
						else if (Input.mousePosition.y <= mouseFirstPosY - JumpThreshold && transform.position.y == groundLevel && !isJumping && !isFalling)
						{
							isSwiping = true;
							swipeCounter = SwipeTime;
							AnimSwipe();
						}
					}
					else
					{
						AnimDirect();
					}
				}
				else if (Input.GetMouseButtonUp(0))
				{
					AnimDirect();
				}



				transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

				#region Jumping
				jumpPosition = transform.position;
				if (isJumping)
				{
					jumpPosition.y += Time.deltaTime * jumpSpeed;
					if (jumpPosition.y >= JumpHeight)
					{
						isJumping = false;
						isFalling = true;
						jumpPosition.y = JumpHeight;

						AnimFinishJump();
					}
				}
				else if (isFalling)
				{
					jumpPosition.y -= Time.deltaTime * jumpSpeed;
					if (jumpPosition.y <= groundLevel)
					{
						isFalling = false;
						jumpPosition.y = groundLevel;
					}
				}
				transform.position = jumpPosition;
				#endregion

				#region Swipe

				if (isSwiping)
				{
					//Swipe Hareketi Bitimi Aksiyonlarý
					if (swipeCounter <= 0)
					{
						isSwiping = false;
						AnimSwipeStop();
						swipeCounter = SwipeTime;

						CC.center = ccCenter;
						CC.height = CapsuleColliderHeight;
					}
					//Swipe Hareketi Aksiyonlarý
					else
					{
						swipeCounter -= Time.deltaTime;

						ccTempCenter = CC.center;
						ccTempCenter.y = ccTempCenter.y - Time.deltaTime * swipeMultiplier / 2 >= ccCenterMinY ? ccTempCenter.y - Time.deltaTime * swipeMultiplier / 2 : ccCenterMinY;
						CC.center = ccTempCenter;

						CC.height = CC.height - Time.deltaTime * swipeMultiplier >= 0 ? CC.height - Time.deltaTime * swipeMultiplier : 1;
					}
				}

				#endregion
			}
			else if (isClimbing)
            {
				if (transform.position.y < groundLevel - 0.5f)
				{
					targetPosition.y = targetPosition.y + climbSpeed * Time.deltaTime > groundLevel - 0.5f ? groundLevel - 0.5f : targetPosition.y + Time.deltaTime * climbSpeed;
					transform.position = targetPosition;
				}
				else if (transform.position.y == groundLevel - 0.5f)
				{
					AnimClimbStop();
					targetPosition.y = targetPosition.y + climbSpeed * Time.deltaTime > groundLevel ? groundLevel : targetPosition.y + Time.deltaTime * climbSpeed;
					transform.position = targetPosition;
				}
				else if (transform.position.y == groundLevel)
                {
					isClimbing = false;
				}
			}
			else if(isLanding)
            {
				
				if(transform.position.y > groundLevel)
                {
					targetPosition.z += Time.deltaTime * fallingForwardSpeed;
					targetPosition.y = targetPosition.y - Time.deltaTime * fallingSpeed < groundLevel ? groundLevel : targetPosition.y - Time.deltaTime * fallingSpeed;
					transform.position = targetPosition;
				}
				else if (transform.position.y == groundLevel)
                {
					isLanding = false;
					AnimFallingStop();
				}	
            }
        }
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(TagObstacle))
		{
			GC.LevelFail();
		}
		else if (other.CompareTag(TagCollectable))
		{
			Destroy(other.gameObject);
			coin++;
			UI.UpdateCoinText(coin.ToString());
			
		}
		else if (other.CompareTag(TagFinish))
		{
			GC.LevelComplete();
		}
		else if (other.CompareTag(TagWallTrigger))
        {
            if (other.gameObject.GetComponent<WallTrigger>().isWallClimbeable)
            {
				isClimbing = true;
				groundLevel += other.gameObject.GetComponent<WallTrigger>().cubeHeight;
				other.gameObject.GetComponent<WallTrigger>().ChangeAttachedCubeTag();
				other.gameObject.GetComponent<WallTrigger>().CloseCollider();
				AnimClimb();
			}
			else if (other.gameObject.GetComponent<WallTrigger>().isWallEnter)
            {
				groundLevel += other.gameObject.GetComponent<WallTrigger>().cubeHeight;
				other.gameObject.GetComponent<WallTrigger>().ChangeAttachedCubeTag();
				other.gameObject.GetComponent<WallTrigger>().CloseCollider();

			}
			else if (other.gameObject.GetComponent<WallTrigger>().isWallExit) 
			{
				isLanding = true;
				groundLevel -= other.gameObject.GetComponent<WallTrigger>().cubeHeight;
				other.gameObject.GetComponent<WallTrigger>().CloseCollider();
				AnimFalling();
			}
        }
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.CompareTag(TagObstacle))
		{
			GC.LevelFail();
		}
	}

	#region End Game Actions

	public void ActionLevelFail()
	{
		// RB.isKinematic = false;
		// RB.constraints = RigidbodyConstraints.None;

		AnimEndLevel();
		AnimFall();

	}

	public void ActionLevelDone()
	{
		AnimEndLevel();
		AnimDance();
	}


    #endregion


    #region Animations
	
	void AnimRun()
    {
		anim.SetBool(animIsMoving, true);
	}

	void AnimJump()
    {
		anim.SetBool(animIsJumping, true);
    }
	void AnimFinishJump()
    {
		anim.SetBool(animIsJumping, false);
    }
	void AnimTurnLeft()
    {
		anim.SetFloat(animHorizontal, -1);
		anim.SetBool(animIsMovingRight, false);
	}
	void AnimTurnRight()
    {
		anim.SetFloat(animHorizontal, 1);
		anim.SetBool(animIsMovingRight, true);
	}
	void AnimDirect()
    {
		anim.SetFloat(animHorizontal, 0);
		anim.SetBool(animIsMovingRight, false);
    }

	void AnimEndLevel()
    {
		anim.SetFloat(animHorizontal, 0);
		anim.SetBool(animIsMoving, false);
		anim.SetBool(animIsMovingRight, false);
		anim.SetBool(animIsJumping, false);
		anim.SetBool(animIsSwipe, false);
	}
	void AnimDance()
    {
		anim.SetBool(animIsDance, true);
    }
	void AnimFall()
    {
		anim.SetBool(animIsFall, true);
    }
	void AnimCoin()
    {
		anim.SetTrigger(animCoke);
		anim.ResetTrigger(animCoke);
	}

	void AnimSwipe()
    {
		anim.SetBool(animIsSwipe,true);
	}
	void AnimSwipeStop()
    {
		anim.SetBool(animIsSwipe, false);
    }
	void AnimClimb()
    {
		anim.SetBool(animIsMoving, false);
		anim.SetBool(animIsMovingRight, false);
		anim.SetBool(animIsJumping, false);
		anim.SetBool(animIsSwipe, false);
		anim.SetBool(animIsClimbing, true);
	}

	void AnimClimbStop()
    {
		anim.SetBool(animIsClimbing, false);
		AnimRun();
    }
	void AnimFalling()
    {
		anim.SetBool(animIsMoving, false);
		anim.SetBool(animIsMovingRight, false);
		anim.SetBool(animIsJumping, false);
		anim.SetBool(animIsSwipe, false);
		anim.SetBool(animIsLanding, true);
	}
	void AnimFallingStop()
	{
		anim.SetBool(animIsLanding, false);
		AnimRun();
	}
	#endregion
}