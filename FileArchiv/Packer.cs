// (C) 2004 André Betz
// http://www.andrebetz.de

using System;
using System.IO;

namespace FileArchiv
{
	/// <summary>
	/// Summary description for Packer.
	/// </summary>
	internal class Packer
	{
		internal delegate void ActualFile(string FileName);
		internal delegate void Error(string FileName);
		private ActualFile CBFunc;
		private Error ErrorFunc;
		private FileStream ArchivFile;

		internal Packer()
		{
		}
		internal void SetCallBacks(ActualFile cbf,Error ef)
		{
			CBFunc = cbf;
			ErrorFunc = ef;
		}

		internal void UnPackFiles(string pathDir,string ArchiveName)
		{
			bool Error = false;

			if(!Error)
			{
				try
				{
					ArchivFile = new FileStream(ArchiveName,FileMode.Open,FileAccess.Read);
				}
				catch
				{
					ErrorFunc("Konnte ArchivFile nicht öffnen");	
					Error = true;
				}
			}

			if(!Error)
			{
				if(!UnPackAllFiles(pathDir))
				{
					Error = true;
				}
			}

			try
			{
				ArchivFile.Close();
			}
			catch
			{
			}

			CBFunc(null);
		}

		internal void PackFiles(string pathDir,string ArchiveName)
		{
			bool Error = false;
			try
			{
				if(File.Exists(ArchiveName))
				{
					File.Delete(ArchiveName);
				}
			}
			catch
			{
				ErrorFunc("Konnte ArchivFile nicht anlegen");
				Error=true;
			}

			if(!Error)
			{
				try
				{
					ArchivFile = new FileStream(ArchiveName,FileMode.OpenOrCreate,FileAccess.Write);
				}
				catch
				{
					ErrorFunc("Konnte ArchivFile nicht öffnen");
					Error=true;
				}
			}

			if(!Error)
			{
				if(!VerzeichnisseDurchgehen(pathDir,true))
				{
					ErrorFunc("Fehler beim Erzeugen des Archives");	
					Error=true;
				}
			}

			if(!Error)
			{
				try
				{
					ArchivFile.Close();
				}
				catch
				{
					Error=true;
				}
			}
			CBFunc(null);
		}

		private bool AddFile(string FileName)
		{
			char[] sperator = {':'};
			string[] splitted = FileName.Split(sperator);
			if(splitted.Length>0)
			{
				FileStream Datei;
				try
				{
					Datei = new FileStream(FileName,FileMode.Open,FileAccess.Read);
				}
				catch
				{
					return false;
				}

				long len = Datei.Length;
				if(!WriteHeader(splitted[1],len))
				{
					return false;
				}

				if(!CopyFile2Archive(Datei,len))
				{
					return false;
				}

				try
				{
					Datei.Close();
				}
				catch
				{
					return false;
				}
				CBFunc(splitted[1]);
				return true;
			}
			return false;
		}

		private bool CopyFile2Archive(FileStream Datei,long len)
		{
			byte[] buffer = new byte[1024*8];
			int offset = 0;
			int readlen = 0;

			while (offset<len)
			{
				try
				{
					readlen = Datei.Read(buffer,0,buffer.Length);
				}
				catch
				{
					return false;
				}

				try
				{
					ArchivFile.Write(buffer,0,readlen);
				}
				catch
				{
					return false;
				}

				offset += readlen;
			}

			ArchivFile.Flush();

			return true;
		}

		private byte[] cpyChar2Byte(char[] chArr)
		{
			byte[] btArr = new byte[chArr.Length];
			for(int i=0;i<btArr.Length;i++)
			{
				btArr[i] = (byte)chArr[i];
			}
			return btArr;
		}

		private bool WriteHeader(string FileName,long len)
		{
			try
			{
				FileName += "\t["+len.ToString()+"]";
				byte[] btAr = cpyChar2Byte(FileName.ToCharArray());
				ArchivFile.Write(btAr,0,btAr.Length);
				ArchivFile.Flush();
			}
			catch
			{
				return false;
			}
			return true;
		}

		private bool VerzeichnisseDurchgehen(string pathDir,bool bOK)
		{
			if(bOK)
			{
				string[] directories = null;
				string[] files = null;
				try
				{
					directories =	Directory.GetDirectories(pathDir);
					files       =	Directory.GetFiles(pathDir);
				}
				catch
				{
				}
				foreach (string str in files) 
				{
					if(!AddFile(str))
					{
						bOK = false;
						break;
					}
				}
				foreach (string str in directories) 
				{
					if(!VerzeichnisseDurchgehen(str,bOK))
					{
						break;
					}
				}
				return true;
			}
			return false;
		}

		private string ReadHeaderFileName()
		{
			long archLen = ArchivFile.Length;
			byte[] buffer = new byte[1];
			string FileName = null;

			while(ArchivFile.Position<archLen)
			{
				try
				{
					ArchivFile.Read(buffer,0,1);
				}
				catch
				{
					return null;
				}
				if(buffer[0]!='\t')
				{
					FileName += (char)buffer[0];
				}
				else
				{
					break;
				}
			}

			return FileName;
		}

		private long ReadHeaderFileLen()
		{
			long archLen = ArchivFile.Length;
			byte[] buffer = new byte[1];
			string FileLenStr = null;
			long FileLen = 0;

			while(ArchivFile.Position<archLen)
			{
				try
				{
					ArchivFile.Read(buffer,0,1);
				}
				catch
				{
					return -1;
				}
				if(buffer[0]=='[')
				{
					break;
				}
			}

			while(ArchivFile.Position<archLen)
			{
				try
				{
					ArchivFile.Read(buffer,0,1);
				}
				catch
				{
					return -1;
				}
				if(buffer[0]!=']')
				{
					FileLenStr+=(char)buffer[0];
				}
				else
				{
					break;
				}
			}

			try
			{
				FileLen = Convert.ToInt32(FileLenStr);
				return FileLen;
			}
			catch
			{
				return -1;
			}
		}

		private bool WriteFileFromArchive(string FileName,long FileLen)
		{
			byte[] buffer = new byte[1024*8];
			int readlen = 0;
			int offset = 0;
			bool Error = false;

			string filepath = Path.GetDirectoryName(FileName);

			if(!Directory.Exists(filepath))
			{
				Directory.CreateDirectory(filepath);
			}

			if(File.Exists(FileName))
			{
				File.Delete(FileName);
			}

			FileStream Datei;
			try
			{
				Datei = new FileStream(FileName,FileMode.Create,FileAccess.Write);
			}
			catch
			{
				return false;
			}
			

			while (offset<FileLen && ArchivFile.Position<ArchivFile.Length && !Error)
			{
				long RestFile = FileLen-offset;
				long LoadLen = RestFile<buffer.Length ? RestFile : buffer.Length;

				try
				{
					readlen = ArchivFile.Read(buffer,0,(int)LoadLen);
				}
				catch
				{
					Error = true;
				}

				try
				{
					Datei.Write(buffer,0,readlen);
				}
				catch
				{
					Error = true;
				}

				offset += readlen;
			}

			Datei.Flush();
			Datei.Close();

			if(Error)
			{
				return false;
			}
			return true;
		}

		private bool UnPackAllFiles(string pathDir)
		{
			long archLen = ArchivFile.Length;
			while(ArchivFile.Position<archLen)
			{
				string FileName = ReadHeaderFileName();
				long   FileLen  = ReadHeaderFileLen();
				if(FileName==null && FileLen<0)
				{
					ErrorFunc("Fehler im ArchivFile Header");	
					return false;
				}
				string FullFileName = pathDir+FileName;
				if(!WriteFileFromArchive(FullFileName,FileLen))
				{
					ErrorFunc("Fehler beim File schreiben");	
					return false;
				}

				CBFunc(FullFileName);
			}

			return false;
		}
	}

}
