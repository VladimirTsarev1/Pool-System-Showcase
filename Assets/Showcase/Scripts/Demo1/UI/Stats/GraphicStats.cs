using System;
using Unity.Profiling;

namespace Showcase.Scripts.Demo1.UI.Stats
{
    public sealed class GraphicStats : IDisposable
    {
        public long GcAllocatedBytesLastFrame { get; private set; }

        private ProfilerRecorder _gcAlloc = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");

        public void Update()
        {
            if (_gcAlloc.Valid && _gcAlloc.Count > 0)
            {
                GcAllocatedBytesLastFrame = _gcAlloc.LastValue;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _gcAlloc.Dispose();
            }
        }
    }
}