using System;
using System.Collections.Generic;
using System.Text;

namespace Scheri.PETPanel.UIComponents;

public interface IMenuConfig
{
    List<Navigate> GetMenuItems();
}

