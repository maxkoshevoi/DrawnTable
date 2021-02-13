using DrawnTableControl.Models;
using System.Collections.Generic;

namespace DrawnTableControl.HeaderHelpers
{
    public interface IHeaderCreator<T, out Index>
    {
        List<DrawnTableHeader> LastGeneratedHeaders { get; }

        T this[int index] { get; }

        Index GetIndexByValue(T value);
    }
}
