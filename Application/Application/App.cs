using System;

using Lenia.Core.Common;

namespace Lenia.Application
{
    public class App
    {
        public string SourceDirectory { get => Directories.DirectoryPrefix; set => Directories.SetDirectoryPrefix(value); }
        public Time CurrentTime { get; set; }

        public App()
        {
            CurrentTime = new Time();
        }

        public void Start()
        {
            CurrentTime.Start();
        }

        public void Update()
        {
            CurrentTime.Update();
        }

        public void FixedUpdate()
        {
            CurrentTime.FixedUpdate();
        }
    }
}
