﻿using System.Collections.Generic;
using libtcod;
using Magecrawl.GameUI.MapEffects;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Requests
{
    public class ShowConeBlast : RequestBase
    {
        private List<Point> m_points;
        private TCODColor m_color;

        public ShowConeBlast(List<Point> points, TCODColor color)
        {
            m_points = points;
            m_color = color;
        }

        internal override void DoRequest(IHandlePainterRequest painter)
        {
            MapEffectsPainter m = painter as MapEffectsPainter;
            if (m != null)
                m.DrawConeBlast(m_points, m_color);
        }
    }
}
