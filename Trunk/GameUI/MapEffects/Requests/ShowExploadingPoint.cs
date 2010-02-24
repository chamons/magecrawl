﻿using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameUI.MapEffects;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class ShowExploadingPoint : RequestBase
    {
        private List<Point> m_path;
        private List<List<Point>> m_blast;
        private EffectDone m_doneDelegate;
        private Color m_color;

        public ShowExploadingPoint(EffectDone doneDelegate, List<Point> path, List<List<Point>> blast, Color color)
        {
            m_doneDelegate = doneDelegate;
            m_path = path;
            m_blast = blast;
            m_color = color;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapEffectsPainter m = painter as MapEffectsPainter;
            if (m != null)
                m.DrawExploadingPointBlast(m_doneDelegate, m_path, m_blast, m_color);
        }
    }
}