/*
 * @Author: iwiniwin
 * @Date: 2020-11-06 00:52:28
 * @LastEditors: iwiniwin
 * @LastEditTime: 2021-05-30 10:38:15
 * @Description: 单例
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URFS
{

    public abstract class Singleton<T> where T : new()
    {
        private static T _instance;
        private static object _lock = new object();
        public static T Instance
        {
            get
            {
                if(_instance == null){
                    lock(_lock){
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }
    }


    public class UnitySingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance{
            get{
                if(_instance == null){
                    _instance = FindObjectOfType(typeof(T)) as T;
                }
                if(_instance == null){
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent(typeof(T)) as T;
                }
                return _instance;
            }
        }

        private void Awake() {
            DontDestroyOnLoad(this.gameObject);
            if(_instance == null){
                _instance = this as T;
            }else{
                Destroy(this.gameObject);
            }
        }

    }

}
