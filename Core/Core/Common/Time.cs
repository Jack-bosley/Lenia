﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lenia.Core.Common
{
    public class Time
    {
        public long DeltaTicks { get; private set; }
        public long FixedDeltaTicks {get; private set;}

        public float DeltaTime => DeltaTicks * 1e-7f;
        public float FixedDeltaTime => FixedDeltaTicks * 1e-7f;

        private long LastUpdateTick { get; set; }
        private long LastFixedUpdateTick {get; set; }

        public DateTime StartTime { get; private set; }

        public void Start()
        {
            StartTime = DateTime.Now;

            DeltaTicks = 0;
            FixedDeltaTicks = 0;
            LastUpdateTick = StartTime.Ticks;
            LastFixedUpdateTick = StartTime.Ticks;
        }

        public void Update()
        {
            DeltaTicks = DateTime.Now.Ticks - LastUpdateTick;
            LastUpdateTick = DateTime.Now.Ticks;
        }

        public void FixedUpdate()
        {
            FixedDeltaTicks = DateTime.Now.Ticks - LastFixedUpdateTick;
            LastFixedUpdateTick = DateTime.Now.Ticks;
        }
    }
}
