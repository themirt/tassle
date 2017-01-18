﻿// --------------------------------------------------------------------------
// <copyright file="ILayoutControl.cs" company="-">
// Copyright (c) 2008-2017 Eser Ozvataf (eser@ozvataf.com). All rights reserved.
// Web: http://eser.ozvataf.com/ GitHub: http://github.com/eserozvataf
// </copyright>
// <author>Eser Ozvataf (eser@ozvataf.com)</author>
// --------------------------------------------------------------------------

//// This program is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
//// 
//// This program is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tasslehoff.Common.Text;
using Tasslehoff.DataStructures.Trees;

namespace Tasslehoff.Layout
{
    /// <summary>
    /// ILayoutControl interface.
    /// </summary>
    public interface ILayoutControl : ITree2D<Guid, ILayoutControl>, ITree3D<ILayoutControl>, IDisposable, ICloneable
    {
        // properties

        /// <summary>
        /// Gets type
        /// </summary>
        /// <value>
        /// Type
        /// </value>
        string Type { get; }

        /// <summary>
        /// Gets description
        /// </summary>
        /// <value>
        /// Description
        /// </value>
        string Description { get; }

        // methods

        /// <summary>
        /// Renders the control.
        /// </summary>
        /// <param name="controller">Controller instance</param>
        /// <returns>
        /// HTML
        /// </returns>
        string Render(Controller controller);

        /// <summary>
        /// Gets children objects filtered by type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Children objects</returns>
        IEnumerable<T> GetChildrenType<T>() where T : ILayoutControl;

        /// <summary>
        /// Serializes control into json
        /// </summary>
        /// <param name="jsonOutputWriter">Json Output Writer</param>
        /// <param name="separateProperties">Put control properties in a separate node</param>
        void Export(MultiFormatOutputWriter jsonOutputWriter, bool separateProperties = false);

        /// <summary>
        /// Serializes control into json
        /// </summary>
        /// <param name="separateProperties">Put control properties in a separate node</param>
        string Export(bool separateProperties = false);

        /// <summary>
        /// Gets properties
        /// </summary>
        /// <returns>List of properties</returns>
        IEnumerable<LayoutControlProperty> GetProperties();
    }
}
