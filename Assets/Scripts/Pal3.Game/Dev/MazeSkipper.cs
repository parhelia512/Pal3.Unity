﻿// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2025, Jiaqi (0x7c13) Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Pal3.Game.Dev
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Command;
    using Command.Extensions;
    using Core.Command;
    using Core.Command.SceCommands;
    using Core.Contract.Constants;
    using Core.DataReader.Scn;
    using Core.Utilities;
    using Engine.Services;
    using Scene;
    using Script;

    public sealed class MazeSkipper
    {
        private readonly IUserVariableStore<ushort, int> _userVariableStore;
        private readonly SceneManager _sceneManager;

        #region Maze skipper commands
        // _0 for entrance, _1 for exit
        #if PAL3
        private readonly Dictionary<string, IList<ICommand>> _skipperCommands = new()
        {
            { "m01_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 242, 41)
            }},
            { "m01_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 193, 260)
            }},
            { "m02_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 397, 50)
            }},
            { "m02_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 405, 287)
            }},
            { "m03_1_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 15, 34)
            }},
            { "m03_1_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 354, 372)
            }},
            { "m03_1_20300", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 22, 30)
            }},
            { "m03_1_20500", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 366, 370)
            }},
            { "m06_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 184, 203)
            }},
            { "m06_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 149, 29)
            }},
            { "m06_4_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 9, 45)
            }},
            { "m06_4_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 233, 31)
            }},
            { "m08_1a_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 100, 88)
            }},
            { "m08_1a_1", new List<ICommand>()
            {
                new SceneLoadCommand("m08", "1b"),
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 101, 24)
            }},
            { "m08_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 51, 83)
            }},
            { "m08_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 170, 28)
            }},
            { "m09_1_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 27, 222)
            }},
            { "m09_1_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 146, 111)
            }},
            { "m10_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 40, 58)
            }},
            { "m10_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 269, 50)
            }},
            { "m10_1_40900", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 238, 46)
            }},
            { "m10_2_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 24, 18)
            }},
            { "m10_2_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 244, 245)
            }},
            { "m10_2_41100", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 239, 196)
            }},
            { "m10_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 197, 18)
            }},
            { "m10_3_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 35, 243)
            }},
            { "m11_1_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 266, 398)
            }},
            { "m11_1_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 361, 471)
            }},
            { "m11_2_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 180, 358)
            }},
            { "m11_2_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 195, 106)
            }},
            { "m15_2_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 227, 169)
            }},
            { "m15_2_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 18, 171)
            }},
            { "m15_c_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 16, 109)
            }},
            { "m15_c_1", new List<ICommand>()
            {
                new SceneLoadCommand("M15", "A"),
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 2, 15)
            }},
            { "m16_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 242, 28)
            }},
            { "m16_1_1", new List<ICommand>()
            {
                new SceneLoadCommand("M16", "6"),
                new ActorSetTilePositionCommand(-1, 28, 62)
            }},
            { "m17_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 81, 92)
            }},
            { "m17_2_1", new List<ICommand>()
            {
                new SceneLoadCommand("M17", "10"),
                new ActorSetTilePositionCommand(-1, 53, 41)
            }},
            { "m18_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 48, 54)
            }},
            { "m18_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 313, 264)
            }},
            { "m19_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 43, 19)
            }},
            { "m19_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 210, 280)
            }},
            { "m19_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 206, 206)
            }},
            { "m19_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 264, 162)
            }},
            { "m19_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 185, 200)
            }},
            { "m19_3_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 37, 8)
            }},
            { "m20_1_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 40, 32)
            }},
            { "m20_1_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 207, 255)
            }},
            { "m20_1_120700", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 72, 48)
            }},
            { "m20_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 73, 39)
            }},
            { "m20_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 14, 69)
            }},
            { "m20_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 33, 21)
            }},
            { "m20_3_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 80, 79)
            }},
            { "m20_4_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 83, 16)
            }},
            { "m20_4_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 16, 52)
            }},
            { "m21_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 23, 71)
            }},
            { "m21_1_1", new List<ICommand>()
            {
                new SceneLoadCommand("M21", "7"),
                new ActorSetTilePositionCommand(-1, 157, 105)
            }},
            { "m22_3_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 285, 18)
            }},
            { "m22_3_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 63, 14)
            }},
            { "m22_3_130600", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 200, 43)
            }},
            { "m22_3_130700", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 245, 75)
            }},
            { "m22_4_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 301, 33)
            }},
            { "m22_4_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 26, 204)
            }},
            { "m23_2_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 157, 70)
            }},
            { "m23_2_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 244, 207)
            }},
            { "m23_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 178, 121)
            }},
            { "m23_3_1", new List<ICommand>()
            {
                new SceneLoadCommand("M23", "4"),
                new ActorSetTilePositionCommand(-1, 87, 7)
            }},
            { "m23_5_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 112, 13)
            }},
            { "m23_5_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 122, 176)
            }},
            { "m24_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 53, 303)
            }},
            { "m24_1_1", new List<ICommand>()
            {
                new SceneLoadCommand("M24", "4"),
                new ActorSetTilePositionCommand(-1, 82, 39)
            }},
            { "m24_4_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 86, 36)
            }},
            { "m24_4_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 144, 13)
            }},
            { "m24_6_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 12, 18)
            }},
            { "m24_6_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 156, 166)
            }},
            { "m25_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 25, 107)
            }},
            { "m25_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 258, 148)
            }},
            { "m25_2_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 21, 112)
            }},
            { "m25_2_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 281, 87)
            }},
        };
        #elif PAL3A
        private readonly Dictionary<string, IList<ICommand>> _skipperCommands = new()
        {
            { "m01_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 49, 284)
            }},
            { "m01_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 56, 103)
            }},
            { "m01_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 53, 137)
            }},
            { "m01_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 107, 30)
            }},
            { "m02_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 132, 152)
            }},
            { "m02_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 104, 49)
            }},
            { "m03_1_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 161, 157)
            }},
            { "m03_1_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 70, 165)
            }},
            { "m03_1_30101", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 162, 150)
            }},
            { "m03_1_30300", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 167, 222)
            }},
            { "m04_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 201, 421)
            }},
            { "m04_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 200, 84)
            }},
            { "m04_1_40200", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 200, 426)
            }},
            { "m04_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 218, 407)
            }},
            { "m04_2_1", new List<ICommand>()
            {
                new SceneLoadCommand("m04", "3"),
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 30, 71)
            }},
            { "m05_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 12, 269)
            }},
            { "m05_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 153, 270)
            }},
            { "m05_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 152, 260)
            }},
            { "m05_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 284, 23)
            }},
            { "m05_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 122, 245)
            }},
            { "m05_3_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 129, 22)
            }},
            { "m05_4_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 122, 229)
            }},
            { "m05_4_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 121, 45)
            }},
            { "m06_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 230, 332)
            }},
            { "m06_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 149, 11)
            }},
            { "m06_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 147, 336)
            }},
            { "m06_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 72, 61)
            }},
            { "m06_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 63, 105)
            }},
            { "m06_3_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 43, 30)
            }},
            { "m07_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 266, 16)
            }},
            { "m07_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 110, 331)
            }},
            { "m07_1_70100", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 261, 10)
            }},
            { "m07_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 226, 120)
            }},
            { "m07_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 24, 122)
            }},
            { "m07_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 171, 77)
            }},
            { "m07_3_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 60, 77)
            }},
            { "m08_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 71, 406)
            }},
            { "m08_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 60, 64)
            }},
            { "m08_1_80200", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 180, 403)
            }},
            { "m08_1_80300", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 162, 338)
            }},
            { "m09_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 78, 395)
            }},
            { "m09_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 100, 126)
            }},
            { "m09_1_80700", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 325, 396)
            }},
            { "m09_1_81300", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 96, 167)
            }},
            { "m09_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 64, 280)
            }},
            { "m09_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 279, 185)
            }},
            { "m09_2_81400", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 267, 137)
            }},
            { "m10_1_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 265, 400)
            }},
            { "m10_1_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 379, 463)
            }},
            { "m10_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 70, 142)
            }},
            { "m10_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 203, 23)
            }},
            { "m11_9_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 85, 45)
            }},
            { "m11_9_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 31, 70)
            }},
            { "m11_8_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 52, 109)
            }},
            { "m11_8_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 90, 90)
            }},
            { "m11_7_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 118, 125)
            }},
            { "m11_7_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 179, 6)
            }},
            { "m11_6_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 103, 169)
            }},
            { "m11_6_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 88, 74)
            }},
            { "m11_5_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 98, 106)
            }},
            { "m11_5_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 188, 78)
            }},
            { "m11_4_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 262, 139)
            }},
            { "m11_4_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 118, 236),
                new PlayerInteractWithObjectCommand(10)
            }},
            { "m12_1_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 126, 231)
            }},
            { "m12_1_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 119, 102)
            }},
            { "m12_1_100300", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 121, 211),
                new PlayerInteractionRequest()
            }},
            { "m12_1_100400", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 126, 231)
            }},
            { "m12_1_110200", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 123, 225)
            }},
            { "m12_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 110, 227)
            }},
            { "m12_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 111, 15)
            }},
            { "m12_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 160, 172)
            }},
            { "m12_3_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 90, 125)
            }},
            { "m13_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 142, 137)
            }},
            { "m13_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 13, 28)
            }},
            { "m13_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 145, 104)
            }},
            { "m13_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 11, 8)
            }},
            { "m13_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 144, 18)
            }},
            { "m13_3_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 12, 102)
            }},
            { "m13_3_120600", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 104, 135)
            }},
            { "m13_4_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 142, 27)
            }},
            { "m13_4_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 11, 122)
            }},
            { "m13_5_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 145, 140)
            }},
            { "m13_5_1", new List<ICommand>()
            {
                new SceneLoadCommand("m13", "6"),
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 36, 6)
            }},
            { "m13_6_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 75, 74)
            }},
            { "m13_6_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 36, 6)
            }},
            { "m14_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 121, 111)
            }},
            { "m14_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 240, 214)
            }},
            { "m14_1_130100", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 150, 152)
            }},
            { "m14_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 122, 96)
            }},
            { "m14_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 228, 203)
            }},
            { "m14_2_130300", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 208, 128)
            }},
            { "m14_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 78, 56)
            }},
            { "m14_3_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 351, 255)
            }},
            { "m15_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 287, 121)
            }},
            { "m15_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 12, 286)
            }},
            { "m15_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 219, 298)
            }},
            { "m15_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 116, 10)
            }},
            { "m16_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 17, 251)
            }},
            { "m16_1_1", new List<ICommand>()
            {
                new SceneLoadCommand("m16", "5"),
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 20, 20)
            }},
            { "m17_1_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 255, 84)
            }},
            { "m17_1_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 342, 155)
            }},
            { "m18_1_0", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 1),
                new ActorSetTilePositionCommand(-1, 107, 321)
            }},
            { "m18_1_1", new List<ICommand>()
            {
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 103, 22)
            }},
            { "m18_2_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 55, 520)
            }},
            { "m18_2_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 129, 106)
            }},
            { "m18_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 98, 238)
            }},
            { "m18_3_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 16, 136)
            }},
            { "m18_4_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 17, 72)
            }},
            { "m18_4_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 128, 119)
            }},
            { "m19_3_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 137, 276)
            }},
            { "m19_3_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 132, 35)
            }},
            { "m19_8_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 15, 16)
            }},
            { "m19_8_1", new List<ICommand>()
            {
                new SceneLoadCommand("m19", "3"),
                new ActorSetNavLayerCommand(-1, 0),
                new ActorSetTilePositionCommand(-1, 137, 276)
            }},
            { "m19_4_0", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 91, 208)
            }},
            { "m19_4_1", new List<ICommand>()
            {
                new ActorSetTilePositionCommand(-1, 92, 92)
            }},
        };
        #endif
        #endregion

        public MazeSkipper(IUserVariableStore<ushort, int> userVariableStore,
            SceneManager sceneManager)
        {
            _userVariableStore = Requires.IsNotNull(userVariableStore, nameof(userVariableStore));
            _sceneManager = Requires.IsNotNull(sceneManager, nameof(sceneManager));
        }

        public bool IsMazeSceneAndHasSkipperCommands(ScnSceneInfo sceneInfo)
        {
            string cmdHashKeyPrefix = GetCommandHashKeyPrefix(sceneInfo);
            return _skipperCommands.Keys.Any(_ => _.StartsWith(cmdHashKeyPrefix, StringComparison.OrdinalIgnoreCase));
        }

        public void PortalToEntrance()
        {
            if (_sceneManager.GetCurrentScene() is not { } currentScene) return;

            foreach (ICommand command in _skipperCommands[GetCommandHashKeyPrefix(currentScene.GetSceneInfo()) + "0"])
            {
                Pal3.Instance.Execute(command);
            }

            Pal3.Instance.Execute(new ActorStopActionAndStandCommand(ActorConstants.PlayerActorVirtualID));
        }

        public void PortalToExitOrNextStoryPoint()
        {
            if (_sceneManager.GetCurrentScene() is not { } currentScene) return;

            // This is how exit button works:
            // If there are predefined commands that has a format of <CityName>_<sceneName>_<mainStoryVarValue>
            // It will check if current main story var is equal to <mainStoryVarValue>, if it does, it will run the commands
            // if no matching pattern found, it will execute the default exit commands (<CityName>_<sceneName>_1)

            int mainStoryVarCurrentValue = _userVariableStore.Get(ScriptConstants.MainStoryVariableId);

            string cmdHashKeyPrefix = GetCommandHashKeyPrefix(currentScene.GetSceneInfo());
            foreach (string commandKey in _skipperCommands.Keys.Where(_ =>
                         _.StartsWith(cmdHashKeyPrefix, StringComparison.OrdinalIgnoreCase) &&
                         !_.EndsWith("_0") &&
                         !_.EndsWith("_1")))
            {
                int mainStoryVarValue = int.Parse(commandKey[cmdHashKeyPrefix.Length..]);
                if (mainStoryVarCurrentValue == mainStoryVarValue)
                {
                    foreach (ICommand command in _skipperCommands[commandKey])
                    {
                        Pal3.Instance.Execute(command);
                    }
                    Pal3.Instance.Execute(new ActorStopActionAndStandCommand(ActorConstants.PlayerActorVirtualID));
                    return;
                }
            }

            foreach (ICommand command in _skipperCommands[cmdHashKeyPrefix + "1"])
            {
                Pal3.Instance.Execute(command);
            }
            Pal3.Instance.Execute(new ActorStopActionAndStandCommand(ActorConstants.PlayerActorVirtualID));
        }

        private string GetCommandHashKeyPrefix(ScnSceneInfo sceneInfo)
        {
            return $"{sceneInfo.CityName}_{sceneInfo.SceneName}_".ToLower();
        }
    }
}