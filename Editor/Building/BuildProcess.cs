using System;
using UnityEngine;

using UnityEditor;

using System.Collections;

namespace fwp.buildor.editor
{

    abstract public class BuildProcess : IDisposable
    {
        public bool IsBusy => _exec != null;

        protected DataBuildSettingProfile profil;

        IEnumerator _exec = null;

        /// <summary>
        /// override to display progress in cancel bar
        /// </summary>
        protected float procProgress = 0f;

        double _time;

        double Delta => EditorApplication.timeSinceStartup - _time;
        string UID => $"process<{GetType().FullName}";

        protected BuildProcess launch(DataBuildSettingProfile profil)
        {
            _time = EditorApplication.timeSinceStartup;

            this.profil = profil;

            _exec = exec();
            EditorApplication.update += update_check_process;

            return this;
        }

        /// <summary>
        /// do something during this process
        /// </summary>
        abstract protected IEnumerator exec();

        /// <summary>
        /// called each editor frame to update coroutine
        /// </summary>
        void update_check_process()
        {
            try
            {
                bool cancel = EditorUtility.DisplayCancelableProgressBar(UID, "processing...", procProgress);

                if (cancel)
                {
                    log("cancelled");
                    stop();
                    return;
                }

                if (_exec != null)
                {
                    if (!_exec.MoveNext())
                    {
                        _exec = null; // done
                        
                        onCompletion();
                        stop();
                    }
                    // log("frame!");
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                stop();
                onFailed();
            }
        }

        void stop()
        {
            _exec = null;
            EditorApplication.update -= update_check_process;
            EditorUtility.ClearProgressBar();

            log("stopped");
        }

        virtual protected void onCompletion()
        {
            log("done!");
            log($"process<{GetType()} took : {Delta} secondes");
        }

        virtual protected void onFailed()
        {
            log("failed!");
        }

        public void Dispose()
        {
            stop();
        }

        protected void log(string msg)
        {
            ulog(Delta.ToString("F3") + " | " + msg, this);
        }

        static public void ulog(string msg, object tar = null)
        {
            Debug.Log((tar != null ? tar.GetType().Name : string.Empty) + " > " + msg);
        }
    }

}
