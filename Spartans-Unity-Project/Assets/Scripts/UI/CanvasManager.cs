using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

using Spartans.Players;
using System;

namespace Spartans.UI{
    public class CanvasManager : MonoBehaviour
    {
        //Page and Canvas manager pattern from lIamAcadamy on Youtube
        [SerializeField] private PageUI _initialPage;
        private Stack<PageUI> _pageStack = new Stack<PageUI>();
        
        public void Init()
        {
            if(_initialPage != null){
                PushPage(_initialPage);
            }else{
                Debug.LogError("CanvasManager Init Error");
            }
            PageUI[] pagesFound = FindObjectsOfType<PageUI>();
            foreach(PageUI page in pagesFound){
                if(page != _initialPage){
                    page.Exit();
                }
            }
        }
        void Update(){
            if(GameManager.Instance != null && GameManager.Instance.activeState != GameManager.States.Lobby)
            {
                //if(PlayerInput.Instance.escape)
                {
                    //print("esc pressed");
                    //PopPage();
                }
            }
            
        }

        public void PushPage(PageUI page){
            //print("pushed " + page.name);
            page.Enter();
            
            if(_pageStack.Count > 0){
                PageUI prevPage = _pageStack.Peek();

                if(prevPage.ExitOnNewPage){
                    prevPage.Exit();
                }
            }
            _pageStack.Push(page);
        }

        public void PopPage()
        {
            if(_pageStack.Count > 1){
                PageUI page = _pageStack.Pop();
                page.Exit();

                PageUI upcomingPage = _pageStack.Peek();
                if(upcomingPage.ExitOnNewPage){
                    upcomingPage.Enter();
                }
            }else{
                Debug.LogWarning("Only 1 PageUI left in the stack");
            }
        }

        public int GetStackCount()
        {
            return _pageStack.Count;
        }
    }
}
