// The project released under MS-PL license https://daxeditor.codeplex.com/license


// Guids.cs
// MUST match guids.h
using System;

namespace DaxEditor
{
    static class GuidList
    {
        public const string guidDaxEditorPkgString = "2da4570a-86fa-43dc-b855-890d9ad96f4c";
        public const string guidDaxEditorCmdSetString = "e5f8052e-fc46-4920-9bb9-a8687ae567ef";
        public const string guidDaxLanguageService = "CB34F6DA-8CEC-4F9D-BDFE-D1B13B795DF7";
        public const string guidDaxOutputPane = "2aeb6ba1-ba1e-4baa-af58-eb1803614e9f";

       public static readonly Guid guidDaxEditorCmdSet = new Guid(guidDaxEditorCmdSetString);
    };
}