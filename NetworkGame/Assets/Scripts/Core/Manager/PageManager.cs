using UnityEngine;
using System.Collections;
using DG.Tweening;
public class PageManager : MonoBehaviour
{
	public enum AnimationType
	{
		NULL,
		LeftToRight,
		RightToLeft,
		TopToBottom,
		BottomToTop,
		MiddleZoomIn,
		MiddleZoomOut,
	}
	static PageManager m_instance;
	public static PageManager Instance
	{
		get
		{
			return m_instance;
		}
	}
			
	BasePage m_currentPage = null;
	BasePage m_nextPage = null;
	public BasePage currentPage
	{
		get
		{
			return m_currentPage;
		}
	}
	
	public static float AnimationDuration = 0.3f;
    public static float AnimationScaleZoomIn = 10.0f;
    public static float AnimationScaleZoomOut = 0.1f;
	
	void Awake()
	{
		m_instance = this;
	}
	
	void AnimationFinish()
	{
		if (m_currentPage!=null)
		{
			m_currentPage.PageDidDisappear();
		}
		m_currentPage = m_nextPage;
		m_currentPage.PageDidAppear();
		m_nextPage = null;
	}
	public void ShowPage(BasePage page, AnimationType type = AnimationType.NULL)
	{

		Debug.Log ("show page");

		//切换页面把当前的对话框关闭
		HideDialog();

		Tweener tweener = null;
		
		m_nextPage = page;
		Util.AddChild (page.gameObject, this.gameObject);
        //Plane panel = page.GetComponent<Plane>();
        //panel.clipOffset = new Vector2(0, 0);
        //panel.SetRect(0, 0, Screen.width, Screen.height);
		if (m_currentPage)
		{
			m_currentPage.PageWillDisappear();
            //Plane currntpanel = m_currentPage.GetComponent<Plane>();
            //currntpanel.clipOffset = new Vector2(0, 0);
		}
		page.PageWillAppear();
		switch (type)
		{
		case AnimationType.NULL:
			if (m_currentPage) 
			{
				m_currentPage.transform.localPosition = new Vector3 (0, -Screen.height, 0);
			}
			page.transform.localPosition=new Vector3(0,0,0);
			AnimationFinish();
			break;
		case AnimationType.LeftToRight:
			if (m_currentPage)
			{
                m_currentPage.transform.localPosition = new Vector3(0, 0, 0);
                m_currentPage.transform.DOLocalMoveX(Screen.width, AnimationDuration);
			}
			page.transform.localPosition=new Vector3(-Screen.width,0,0);
			tweener = page.transform.DOLocalMoveX(0, AnimationDuration);
			
			break;
		case AnimationType.RightToLeft:
			if (m_currentPage)
			{
				m_currentPage.transform.localPosition=new Vector3(0,0,0);
				m_currentPage.transform.DOLocalMoveX(-Screen.width, AnimationDuration);
			}
			page.transform.localPosition=new Vector3(Screen.width,0,0);
			tweener = page.transform.DOLocalMoveX(0, AnimationDuration);
			break;
		case AnimationType.TopToBottom:
			if (m_currentPage)
			{
				m_currentPage.transform.localPosition=new Vector3(0,0,0);
				m_currentPage.transform.DOLocalMoveY(Screen.height, AnimationDuration);
			}
			page.transform.localPosition=new Vector3(0,-Screen.height,0);
			tweener = page.transform.DOLocalMoveY(0, AnimationDuration);
			break;
		case AnimationType.BottomToTop:
			if (m_currentPage)
			{
				m_currentPage.transform.localPosition=new Vector3(0,0,0);
				m_currentPage.transform.DOLocalMoveY(-Screen.height, AnimationDuration);
			}
			page.transform.localPosition=new Vector3(0,Screen.height,0);
			tweener = page.transform.DOLocalMoveY(0, AnimationDuration);
			break;
		case AnimationType.MiddleZoomIn:
			if (m_currentPage)
			{
				m_currentPage.transform.localPosition = new Vector3 (0, -Screen.height, 0);
			}
			page.transform.localPosition=new Vector3(0,0,0);
			page.transform.localScale=new Vector3(AnimationScaleZoomIn,AnimationScaleZoomIn,1);
			tweener = page.transform.DOScale(new Vector3(1,1,1), AnimationDuration);
			break;
		case AnimationType.MiddleZoomOut:
			if (m_currentPage)
			{
				m_currentPage.transform.localPosition = new Vector3 (0, -Screen.height, 0);
			}
			page.transform.localPosition=new Vector3(0,0,0);
			page.transform.localScale=new Vector3(AnimationScaleZoomOut,AnimationScaleZoomOut,1);
			tweener = page.transform.DOScale(new Vector3(1,1,1), AnimationDuration);
			break;
		default:
			break;
		}
		if (tweener!=null)
		{
			tweener.OnComplete(AnimationFinish);
		}
	}
	
	BasePage m_currentDialog = null;
	BasePage m_nextDialog = null;
	public BasePage currentDialog
	{
		get
		{
			return m_currentDialog;
		}
	}
	
	void DialogAnimationFinish()
	{
		if (m_currentDialog)
		{
			m_currentDialog.PageDidDisappear();
		}
		m_currentDialog = m_nextDialog;
		m_currentDialog.PageDidAppear();
	}

    public Tweener ShowDialog(BasePage dialog, AnimationType type = AnimationType.NULL)
	{
		Tweener tweener = null;
		
		m_nextDialog = dialog;

		//屏蔽page点击事件
		dialog.gameObject.AddComponent<BoxCollider>();
		BoxCollider collider = dialog.gameObject.GetComponent<BoxCollider>();
		collider.size = new Vector3 (Screen.width, Screen.height, 1);

		Util.AddChild (dialog.gameObject, this.gameObject);
		if (m_currentDialog)
		{
            m_currentDialog.PageWillDisappear();
			m_currentDialog.transform.localPosition = new Vector3 (0, -Screen.height, 0);
            m_currentDialog.PageDidDisappear();
		}
		dialog.PageWillAppear();
		
		switch (type)
		{
		case AnimationType.NULL:
			dialog.transform.localPosition=new Vector3(0,0,0);
			DialogAnimationFinish();
			break;
		case AnimationType.LeftToRight:
			dialog.transform.localPosition=new Vector3(-Screen.width,0,0);
			tweener = dialog.transform.DOLocalMoveX(0, AnimationDuration);
			break;
		case AnimationType.RightToLeft:
			dialog.transform.localPosition=new Vector3(Screen.width,0,0);
			tweener = dialog.transform.DOLocalMoveX(0, AnimationDuration);
			break;
		case AnimationType.TopToBottom:
			dialog.transform.localPosition=new Vector3(0,-Screen.height,0);
			tweener = dialog.transform.DOLocalMoveY(0, AnimationDuration);
			break;
		case AnimationType.BottomToTop:
			dialog.transform.localPosition=new Vector3(0,Screen.height,0);
			tweener = dialog.transform.DOLocalMoveY(0, AnimationDuration);
			break;
		case AnimationType.MiddleZoomIn:
			dialog.transform.localPosition=new Vector3(0,0,0);
			dialog.transform.localScale=new Vector3(AnimationScaleZoomIn,AnimationScaleZoomIn,1);
			tweener = dialog.transform.DOScale(new Vector3(1,1,1), AnimationDuration);
			break;
		case AnimationType.MiddleZoomOut:
			dialog.transform.localPosition=new Vector3(0,0,0);
			dialog.transform.localScale=new Vector3(AnimationScaleZoomOut,AnimationScaleZoomOut,1);
			tweener = dialog.transform.DOScale(new Vector3(1,1,1), AnimationDuration);
			break;
		default:
			break;
		}
		
		if (tweener!=null)
		{
			tweener.OnComplete(DialogAnimationFinish);
		}

        return tweener;
	}

    public void HideDialog(AnimationType type = AnimationType.NULL)
	{
		if (!m_currentDialog) 
		{
            return;
		}
			
        Tweener tweener = null;

        m_currentDialog.PageWillDisappear();
        m_currentDialog.transform.localPosition = new Vector3(0, -Screen.height, 0);

        switch (type)
        {
            case AnimationType.NULL:
                m_currentDialog.transform.localPosition = new Vector3(0, 0, 0);
                DialogAnimationFinish();
                break;
            case AnimationType.LeftToRight:
                m_currentDialog.transform.localPosition = new Vector3(0, 0, 0);
                tweener = m_currentDialog.transform.DOLocalMoveX(Screen.width, AnimationDuration);
                break;
            case AnimationType.RightToLeft:
                m_currentDialog.transform.localPosition = new Vector3(0, 0, 0);
                tweener = m_currentDialog.transform.DOLocalMoveX(-Screen.width, AnimationDuration);
                break;
            case AnimationType.TopToBottom:
                m_currentDialog.transform.localPosition = new Vector3(0, 0, 0);
                tweener = m_currentDialog.transform.DOLocalMoveY(-Screen.height, AnimationDuration);
                break;
            case AnimationType.BottomToTop:
                m_currentDialog.transform.localPosition = new Vector3(0, 0, 0);
                tweener = m_currentDialog.transform.DOLocalMoveY(Screen.height, AnimationDuration);
                break;
            case AnimationType.MiddleZoomIn:
                m_currentDialog.transform.localPosition = new Vector3(0, 0, 0);
                m_currentDialog.transform.localScale = new Vector3(1, 1, 1);
                tweener = m_currentDialog.transform.DOScale(new Vector3(AnimationScaleZoomIn, AnimationScaleZoomIn, 1), AnimationDuration);
                break;
            case AnimationType.MiddleZoomOut:
                m_currentDialog.transform.localPosition = new Vector3(0, 0, 0);
                m_currentDialog.transform.localScale = new Vector3(1, 1, 1);
                tweener = m_currentDialog.transform.DOScale(new Vector3(AnimationScaleZoomOut, AnimationScaleZoomOut, 1), AnimationDuration);
                break;
            default:
                break;
        }
			
        if (tweener != null)
        {
            tweener.OnComplete(DialogAnimationFinish);
        }
	}

	public void CloseDialog()
	{
		if (m_currentDialog) 
		{
			Destroy(m_currentDialog.gameObject);
		}
		//DialogAnimationFinish();
	}
}
