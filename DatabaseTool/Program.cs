using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.AddInfo();
        }

        public void AddInfo()
        {
            string[] x = System.IO.Directory.GetFiles(@"D:\CloudMusic", "*.mp3", SearchOption.AllDirectories);
            for(int i = 0; i< x.Length; i++)
            {
                SongInfo songInfo = GetSong(x[i]);
                int artistId = AddNewArtist(songInfo.artist);
                int albumId = AddNewAlbum(songInfo.album, songInfo.image);
                AddNewSong(songInfo, artistId, albumId);
            }
        }

        public SongInfo GetSong(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            TagLib.File songFileInfo = TagLib.File.Create(path);
            string artist = songFileInfo.Tag.FirstPerformer;
            string title = songFileInfo.Tag.Title;
            string album = songFileInfo.Tag.Album;
            byte[] image = null;
            if (songFileInfo.Tag.Pictures.Length >= 1)
            {
                //tag中的图片信息为byte数组，需要用函数进行转化
                image = songFileInfo.Tag.Pictures[0].Data.Data;
                //pictureBox2.Image = ReturnPhoto(bin);//转化函数
            }
            byte[] file = new byte[fileInfo.Length];
            using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fs.Read(file, 0, file.Length);
                fs.Flush();
            }
            return new SongInfo() { name = title, artist = artist, album = album, file = file, image = image };
        }
        /// <summary>
        /// 添加新歌手，如果已经有相同名字的不添加
        /// </summary>
        /// <param name="name"></param>
        /// <returns>歌手id</returns>
        public int AddNewArtist(string name)
        {
            using (playerdbEntities context = new playerdbEntities())
            {
                
                var query = (from artist in context.artists
                             where artist.name == name
                             select artist.id).SingleOrDefault();
                if (query <= 0)
                {
                    context.artists.Add(new artist() { name = name });
                    context.Database.ExecuteSqlCommand("SET NAMES UTF8");
                    context.SaveChanges();
                    return (from artist in context.artists
                          where artist.name == name
                          select artist.id).SingleOrDefault();
                }
                else
                {
                    return query;
                }
            }
        }
        /// <summary>
        /// 添加新专辑，如果已经有相同名字的不添加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="image"></param>
        /// <returns>专辑的id</returns>
        public int AddNewAlbum(string name,byte[] image)
        {
            using (playerdbEntities context = new playerdbEntities())
            {
                var query = (from album in context.albums
                             where album.name == name
                             select album.id).SingleOrDefault();
                if (query == 0)
                {
                    context.albums.Add(new album() { name = name , image = image});
                    context.Database.ExecuteSqlCommand("SET NAMES UTF8");
                    context.SaveChanges();
                    return (from album in context.albums
                          where album.name == name
                          select album.id).SingleOrDefault();
                }
                else
                {
                    return query;
                }
            }
        }
        /// <summary>
        /// 添加新歌曲，如果已经有相同名字的不添加
        /// </summary>
        /// <param name="songInfo"></param>
        /// <param name="artistId"></param>
        /// <param name="albumId"></param>
        /// <returns>歌曲id</returns>
        public int AddNewSong(SongInfo songInfo, int artistId, int albumId)
        {
            
            using (playerdbEntities context = new playerdbEntities())
            {
                var query = (from song in context.songs
                             where song.name == songInfo.name
                             select song.id).SingleOrDefault();
                if (query == 0)
                {
                    context.songs.Add(
                        new song()
                        {
                            name = songInfo.name,
                            artist_id = artistId,
                            album_id = albumId,
                            file = songInfo.file,
                            file_length = songInfo.file.Length,
                            image = songInfo.image
                        });
                    context.Database.ExecuteSqlCommand("SET NAMES UTF8");
                    context.SaveChanges();
                    return (from song in context.songs
                            where song.name == songInfo.name
                            select song.id).SingleOrDefault();
                }
                else
                {
                    return query;
                }
            }
        }

        public class SongInfo
        {
            public string name { get; set; }
            public string artist { get; set; }
            public string album { get; set; }
            public byte[] file { get; set; }
            public byte[] image { get; set; }
        }

        //public void AddFileInfo()
        //{
        //    DirectoryInfo directory = new DirectoryInfo(@"D:\CloudMusic\");

        //    using(FileStream fs = new FileStream("sample.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        foreach (FileInfo fileInfo in directory.GetFiles("*.mp3"))
        //        {
        //            long length = fileInfo.Length;
        //            byte[] bytes = new byte[length];
        //            fs.Read(bytes, 0, bytes.Length);
        //            fs.Flush();
        //            using (var context = new playerdbEntities())
        //            {
        //                file file = new file
        //                {
        //                    name = fileInfo.Name,
        //                    length = length,
        //                    file1 = bytes
        //                };
        //                context.files.Add(file);
        //                context.SaveChanges();
        //            }
        //        }
        //    }
        //}
    }
}
