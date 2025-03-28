// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2025, Jiaqi (0x7c13) Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Pal3.Core.Command.SceCommands
{
    [SceCommand(119, "显示客栈入住菜单" +
                    "参数：客栈脚本文件名，可以入住脚本ID，不可以入住脚本ID，住宿脚本ID")]
    public sealed class UIShowHotelMenuCommand : ICommand
    {
        public UIShowHotelMenuCommand(string hotelScriptName,
            int canRestScriptId,
            int cannotRestScriptId,
            int afterRestScriptId)
        {
            HotelScriptName = hotelScriptName;
            CanRestScriptId = canRestScriptId;
            CannotRestScriptId = cannotRestScriptId;
            AfterRestScriptId = afterRestScriptId;
        }

        public string HotelScriptName { get; }
        public int CanRestScriptId { get; }
        public int CannotRestScriptId { get; }
        public int AfterRestScriptId { get; }
    }
}