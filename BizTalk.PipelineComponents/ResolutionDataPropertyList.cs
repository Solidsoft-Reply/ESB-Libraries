// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolutionDataPropertyList.cs" company="Solidsoft Reply Ltd.">
//   Copyright 2015 Solidsoft Reply Limited.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing.Design;

    /// <summary>
    /// Type to handle list of resolution data properties.
    /// </summary>
    [Editor(typeof(ResolutionDataPropertyEditor), typeof(UITypeEditor))]
    public class ResolutionDataPropertyList : IList<string>
    {
        /// <summary>
        /// The list of resolution data properties.
        /// </summary>
        private readonly IList<string> propertyList = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionDataPropertyList"/> class.
        /// </summary>
        public ResolutionDataPropertyList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionDataPropertyList"/> class.
        /// </summary>
        /// <param name="propertyList">The property list.</param>
        public ResolutionDataPropertyList(IList<string> propertyList)
        {
            this.propertyList = propertyList;
        }
        
        /// <summary>
        /// Gets the number of elements contained in the ICollection&lt;T&gt;.
        /// </summary>
        public int Count
        {
            get
            {
                return this.propertyList.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the ICollection&lt;T&gt; is read-only.
        /// </summary>
        public bool IsReadOnly 
        {
            get
            {
                return this.propertyList.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public string this[int index]
        {
            get
            {
                return this.propertyList[index];
            }

            set
            {
                this.propertyList[index] = value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return this.propertyList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the ICollection&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to add to the ICollection&lt;T&gt;.</param>
        public void Add(string item)
        {
            this.propertyList.Add(item);
        }

        /// <summary>
        /// Removes all items from the ICollection&lt;T&gt;.
        /// </summary>
        public void Clear()
        {
            this.propertyList.Clear();
        }

        /// <summary>
        /// Determines whether the ICollection&lt;T&gt; contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the ICollection&lt;T&gt;.</param>
        /// <returns>true if item is found in the ICollection&lt;T&gt;; otherwise, false.</returns>
        public bool Contains(string item)
        {
            return this.propertyList.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the ICollection&lt;T&gt; to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied
        /// from ICollection&lt;T&gt;. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(string[] array, int arrayIndex)
        {
            this.propertyList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the ICollection&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to remove from the ICollection&lt;T&gt;.</param>
        /// <returns>true if item was successfully removed from the ICollection&lt;T&gt; 
        /// otherwise, false. This method also returns false if item is not found in 
        /// the original ICollection&lt;T&gt;.</returns>
        public bool Remove(string item)
        {
            return this.propertyList.Remove(item);
        }
        
        /// <summary>
        /// Determines the index of a specific item in the IList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the IList&lt;T&gt;.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(string item)
        {
            return this.propertyList.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the IList&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the IList&lt;T&gt;.</param>
        public void Insert(int index, string item)
        {
            this.propertyList.Insert(index, item);
        }

        /// <summary>
        /// Removes the IList&lt;T&gt; item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            this.propertyList.RemoveAt(index);
        }
    }
}
