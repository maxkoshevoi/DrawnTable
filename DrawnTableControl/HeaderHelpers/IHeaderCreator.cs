using DrawnTableControl.Models;
using System.Collections.Generic;

namespace DrawnTableControl.HeaderHelpers
{
    public interface IHeaderCreator<T, Index>
    {
        List<DrawnTableHeader> LastGeneratedHeaders { get; }

        T GetValueByIndex(int index);

        Index GetIndexByValue(T value);
    }
}
