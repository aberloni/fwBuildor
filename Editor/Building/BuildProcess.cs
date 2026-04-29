using System;
using UnityEngine;

using UnityEditor;

using System.Collections;

namespace fwp.buildor.editor
{

    abstract public class BuildProcess
    {
        protected DataBuildSettingProfile profil;

        public Action onCompletion;

        IEnumerator _exec = null;

        double time;

        protected BuildProcess launch(DataBuildSettingProfile profil)
        {
            time = EditorApplication.timeSinceStartup;

            this.profil = profil;

            EditorApplication.update += update_check_process;

            // log("exec..start");
            _exec = exec();

            return this;
        }

        abstract protected IEnumerator exec();

        void update_check_process()
        {
            if (_exec != null)
            {
                if (!_exec.MoveNext())
                {
                    // log("exec..finished");
                    _exec = null;
                }
                else
                {
                    // log("exec..");
                    return;
                }
            }

            // log("exec..done");
            onCompletion?.Invoke();
            EditorApplication.update -= update_check_process;
        }

        protected void log(string msg)
        {
            var dt = EditorApplication.timeSinceStartup - time;
            ulog(dt + " | " + msg, this);
        }

        static public void ulog(string msg, object tar = null)
        {
            Debug.Log((tar != null ? tar.GetType().Name : string.Empty) + " > " + msg);
        }
    }

}
