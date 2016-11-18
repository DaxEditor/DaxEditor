/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;

namespace Babel
{
	public class Methods : Microsoft.VisualStudio.Package.Methods
	{
		IList<Method> methods;
		public Methods(IList<Method> methods)
		{
			this.methods = methods;
		}

		public override int GetCount()
		{
			return methods.Count;
		}

		public override string GetName(int index)
		{
			return methods[index].Name;
		}

		public override string GetDescription(int index)
		{
			return methods[index].Description;
		}

		public override string GetType(int index)
		{
			return methods[index].Type;
		}

		public override int GetParameterCount(int index)
		{
			return (methods[index].Parameters == null) ? 0 : methods[index].Parameters.Count;
		}

		public override void GetParameterInfo(int index, int paramIndex, out string name, out string display, out string description)
		{
			Parameter parameter = methods[index].Parameters[paramIndex];
			name = parameter.Name;
			display = parameter.Display;
			description = parameter.Description;
		}
	}
}