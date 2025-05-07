using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

namespace fwp.buildor
{
	/// <summary>
	/// wrapper around how to write on hardware
	/// </summary>
	abstract public class SaveFileSystem
	{

		static SaveFileSystem _fsys;
		static public SaveFileSystem fsys
		{
			get
			{
				if (_fsys == null)
				{
					if (SystemDetection.isDesktop())
					{
						_fsys = new SaveFileSystemWindows();
					}
					else if (SystemDetection.isSwitch())
					{
						_fsys = new SaveFileSystemWindows();
					}
				}

				if (_fsys == null)
				{
					Debug.LogError("platform saving not supported ?");
				}

				return _fsys;
			}
		}

		public static byte[] serializeObject(object obj)
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter bf = new BinaryFormatter();

			try
			{
				bf.Serialize(stream, obj);
			}
			catch (Exception e)
			{
				Debug.LogError("issue during serialization : " + obj);
				Debug.LogError(e);
				return null;
			}

			return stream.GetBuffer();
		}

		public static object deserializeObject(byte[] buffer)
		{
			MemoryStream stream = new MemoryStream(buffer);
			BinaryFormatter bf = new BinaryFormatter();

			object ret = null;
			try
			{
				ret = bf.Deserialize(stream);
			}
			catch (Exception e)
			{
				Debug.LogWarning("issue during DE-serialization : " + buffer);
				Debug.LogWarning(e);
				ret = null;
			}

			return ret;
		}

		abstract public bool checkFolderExists(string path);

		abstract public bool writeBytes(string path, byte[] bytes);
		abstract public bool readBytes(string path, out byte[] result);
		abstract public bool delete(string absPath);

		abstract public bool hasPhysicalFile(string absPath);

	}

}
