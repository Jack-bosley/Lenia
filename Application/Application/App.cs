using System;

using Lenia.Core.Common;
using Lenia.Core.Rendering;

namespace Lenia.Application
{
    public class App
    {
        public string SourceDirectory { get => Directories.DirectoryPrefix; set => Directories.SetDirectoryPrefix(value); }
        public Time CurrentTime { get; set; }

        public Mesh<TexturedPositionVertex> renderMesh;

        public App()
        {
            CurrentTime = new Time();
        }

        public void Start()
        {
            CurrentTime.Start();

            Console.WriteLine($"Started Application at {CurrentTime.StartTime}");
            Console.WriteLine($"Source {SourceDirectory}");

            float w = 1f;

            renderMesh = new Mesh<TexturedPositionVertex>();
            renderMesh.Vertices = new TexturedPositionVertex[]
            {
                new TexturedPositionVertex(-w, -w, 0, 0, 0),
                new TexturedPositionVertex( w, -w, 0, 1, 0),
                new TexturedPositionVertex(-w,  w, 0, 0, 1),
                new TexturedPositionVertex (w,  w, 0, 1, 1),
            };
            renderMesh.Indices = new uint[]
            {
                0, 1, 2,
                2, 1, 3,
            };
            renderMesh.Update();
        }

        public void Update()
        {
            CurrentTime.Update();

            renderMesh.Draw();
        }

        public void FixedUpdate()
        {
            CurrentTime.FixedUpdate();
        }
    }
}
