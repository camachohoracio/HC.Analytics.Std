#region

using System;
using System.Text;
using HC.Analytics.Colt.objectAlgo;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Colt.doubleAlgo
{
    /*
    Copyright ï¿½ 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package doublealgo;

    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    ////import DoubleMatrix3D;
    ////import AbstractFormatter;
    ////import AbstractMatrix1D;
    ////import AbstractMatrix2D;
    ////import DenseDoubleMatrix1D;
    ////import Former;
    /** 
    Flexible, well human readable matrix print formatting; By default decimal point aligned. Build on top of the C-like <i>sprintf</i> functionality 
      provided by the {@link corejava.Format} class written by Cay Horstmann.
      Currenly works on 1-d, 2-d and 3-d matrices.
      Note that in most cases you will not need to get familiar with this class; just call <tt>matrix.ToString()</tt> and be happy with the default formatting.
      This class is for advanced requirements.
    <p> Can't exactly remember the syntax of printf format strings? See {@link corejava.Format} 
      or <a href="http://www.braju.com/docs/index.html">Henrik 
      Nordberg's documentation</a>, or the <a href="http://www.dinkumware.com/htm_cl/lib_prin.html#Print%20Functions">Dinkumware's 
      C Library Reference</a>.
  
    <p><b>Examples:</b>
    <p>
    Examples demonstrate usage on 2-d matrices. 1-d and 3-d matrices formatting works very similar.
    <table border="1" cellspacing="0">
      <tr align="center"> 
        <td>Original matrix</td>
      </tr>
      <tr> 
        <td> 
	  
          <p><tt>double[,] values = {<br>
            {3, 0, -3.4, 0},<br>
            {5.1 ,0, +3.0123456789, 0}, <br>
            {16.37, 0.0, 2.5, 0}, <br>
            {-16.3, 0, -3.012345678E-4, -1},<br>
            {1236.3456789, 0, 7, -1.2}<br>
            };<br>
            matrix = new DenseDoubleMatrix2D(values);</tt></p>
        </td>
      </tr>
    </table>
    <p>&nbsp;</p>
    <table border="1" cellspacing="0">
      <tr align="center"> 
        <td><tt>format</tt></td>
        <td valign="top"><tt>Formatter.ToString(matrix);</tt></td>
        <td valign="top"><tt>Formatter.toSourceCode(matrix);</tt></td>
      </tr>
      <tr> 
        <td><tt>%G </tt><br>
          (default)</td>
        <td align="left" valign="top"><tt>5&nbsp;x&nbsp;4&nbsp;matrix<br>
          &nbsp;&nbsp;&nbsp;3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;-3.4&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;&nbsp;<br>
          &nbsp;&nbsp;&nbsp;5.1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;&nbsp;3.012346&nbsp;&nbsp;0&nbsp;&nbsp;<br>
          &nbsp;&nbsp;16.37&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;&nbsp;2.5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;&nbsp;<br>
          &nbsp;-16.3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;-0.000301&nbsp;-1&nbsp;&nbsp;<br>
          1236.345679&nbsp;0&nbsp;&nbsp;7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-1.2 
          </tt></td>
        <td align="left" valign="top"><tt>{<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;&nbsp;3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0,&nbsp;-3.4&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;&nbsp;0&nbsp;&nbsp;},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;&nbsp;5.1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0,&nbsp;&nbsp;3.012346,&nbsp;&nbsp;0&nbsp;&nbsp;},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;16.37&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0,&nbsp;&nbsp;2.5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;&nbsp;0&nbsp;&nbsp;},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;-16.3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0,&nbsp;-0.000301,&nbsp;-1&nbsp;&nbsp;},<br>
          &nbsp;&nbsp;&nbsp;{1236.345679,&nbsp;0,&nbsp;&nbsp;7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;-1.2}<br>
          }; </tt></td>
      </tr>
      <tr> 
        <td><tt>%1.10G</tt></td>
        <td align="left" valign="top"><tt>5&nbsp;x&nbsp;4&nbsp;matrix<br>
          &nbsp;&nbsp;&nbsp;3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;-3.4&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;&nbsp;<br>
          &nbsp;&nbsp;&nbsp;5.1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;&nbsp;3.0123456789&nbsp;&nbsp;0&nbsp;&nbsp;<br>
          &nbsp;&nbsp;16.37&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;&nbsp;2.5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;&nbsp;<br>
          &nbsp;-16.3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0&nbsp;-0.0003012346&nbsp;-1&nbsp;&nbsp;<br>
          1236.3456789&nbsp;0&nbsp;&nbsp;7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-1.2 
          </tt></td>
        <td align="left" valign="top"><tt>{<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;&nbsp;3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0,&nbsp;-3.4&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;&nbsp;0&nbsp;&nbsp;},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;&nbsp;5.1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0,&nbsp;&nbsp;3.0123456789,&nbsp;&nbsp;0&nbsp;&nbsp;},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;16.37&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0,&nbsp;&nbsp;2.5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;&nbsp;0&nbsp;&nbsp;},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;-16.3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0,&nbsp;-0.0003012346,&nbsp;-1&nbsp;&nbsp;},<br>
          &nbsp;&nbsp;&nbsp;{1236.3456789,&nbsp;0,&nbsp;&nbsp;7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;-1.2}<br>
          }; </tt></td>
      </tr>
      <tr> 
        <td><tt>%f</tt></td>
        <td align="left" valign="top"> <tt> 5&nbsp;x&nbsp;4&nbsp;matrix<br>
          &nbsp;&nbsp;&nbsp;3.000000&nbsp;0.000000&nbsp;-3.400000&nbsp;&nbsp;0.000000<br>
          &nbsp;&nbsp;&nbsp;5.100000&nbsp;0.000000&nbsp;&nbsp;3.012346&nbsp;&nbsp;0.000000<br>
          &nbsp;&nbsp;16.370000&nbsp;0.000000&nbsp;&nbsp;2.500000&nbsp;&nbsp;0.000000<br>
          &nbsp;-16.300000&nbsp;0.000000&nbsp;-0.000301&nbsp;-1.000000<br>
          1236.345679&nbsp;0.000000&nbsp;&nbsp;7.000000&nbsp;-1.200000 </tt> </td>
        <td align="left" valign="top"><tt> {<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;&nbsp;3.000000,&nbsp;0.000000,&nbsp;-3.400000,&nbsp;&nbsp;0.000000},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;&nbsp;5.100000,&nbsp;0.000000,&nbsp;&nbsp;3.012346,&nbsp;&nbsp;0.000000},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;16.370000,&nbsp;0.000000,&nbsp;&nbsp;2.500000,&nbsp;&nbsp;0.000000},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;-16.300000,&nbsp;0.000000,&nbsp;-0.000301,&nbsp;-1.000000},<br>
          &nbsp;&nbsp;&nbsp;{1236.345679,&nbsp;0.000000,&nbsp;&nbsp;7.000000,&nbsp;-1.200000}<br>
          }; </tt> </td>
      </tr>
      <tr> 
        <td><tt>%1.2f</tt></td>
        <td align="left" valign="top"><tt>5&nbsp;x&nbsp;4&nbsp;matrix<br>
          &nbsp;&nbsp;&nbsp;3.00&nbsp;0.00&nbsp;-3.40&nbsp;&nbsp;0.00<br>
          &nbsp;&nbsp;&nbsp;5.10&nbsp;0.00&nbsp;&nbsp;3.01&nbsp;&nbsp;0.00<br>
          &nbsp;&nbsp;16.37&nbsp;0.00&nbsp;&nbsp;2.50&nbsp;&nbsp;0.00<br>
          &nbsp;-16.30&nbsp;0.00&nbsp;-0.00&nbsp;-1.00<br>
          1236.35&nbsp;0.00&nbsp;&nbsp;7.00&nbsp;-1.20 </tt></td>
        <td align="left" valign="top"><tt>{<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;&nbsp;3.00,&nbsp;0.00,&nbsp;-3.40,&nbsp;&nbsp;0.00},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;&nbsp;5.10,&nbsp;0.00,&nbsp;&nbsp;3.01,&nbsp;&nbsp;0.00},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;16.37,&nbsp;0.00,&nbsp;&nbsp;2.50,&nbsp;&nbsp;0.00},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;-16.30,&nbsp;0.00,&nbsp;-0.00,&nbsp;-1.00},<br>
          &nbsp;&nbsp;&nbsp;{1236.35,&nbsp;0.00,&nbsp;&nbsp;7.00,&nbsp;-1.20}<br>
          }; </tt></td>
      </tr>
      <tr> 
        <td><tt>%0.2e</tt></td>
        <td align="left" valign="top"><tt>5&nbsp;x&nbsp;4&nbsp;matrix<br>
          &nbsp;3.00e+000&nbsp;0.00e+000&nbsp;-3.40e+000&nbsp;&nbsp;0.00e+000<br>
          &nbsp;5.10e+000&nbsp;0.00e+000&nbsp;&nbsp;3.01e+000&nbsp;&nbsp;0.00e+000<br>
          &nbsp;1.64e+001&nbsp;0.00e+000&nbsp;&nbsp;2.50e+000&nbsp;&nbsp;0.00e+000<br>
          -1.63e+001&nbsp;0.00e+000&nbsp;-3.01e-004&nbsp;-1.00e+000<br>
          &nbsp;1.24e+003&nbsp;0.00e+000&nbsp;&nbsp;7.00e+000&nbsp;-1.20e+000 </tt></td>
        <td align="left" valign="top"><tt>{<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;3.00e+000,&nbsp;0.00e+000,&nbsp;-3.40e+000,&nbsp;&nbsp;0.00e+000},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;5.10e+000,&nbsp;0.00e+000,&nbsp;&nbsp;3.01e+000,&nbsp;&nbsp;0.00e+000},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;1.64e+001,&nbsp;0.00e+000,&nbsp;&nbsp;2.50e+000,&nbsp;&nbsp;0.00e+000},<br>
          &nbsp;&nbsp;&nbsp;{-1.63e+001,&nbsp;0.00e+000,&nbsp;-3.01e-004,&nbsp;-1.00e+000},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;1.24e+003,&nbsp;0.00e+000,&nbsp;&nbsp;7.00e+000,&nbsp;-1.20e+000}<br>
          }; </tt></td>
      </tr>
      <tr> 
        <td><tt>null</tt></td>
        <td align="left" valign="top"><tt>5&nbsp;x&nbsp;4&nbsp;matrix <br>
          &nbsp;&nbsp;&nbsp;3.0&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0.0&nbsp;-3.4&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0.0<br>
          &nbsp;&nbsp;&nbsp;5.1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0.0&nbsp;&nbsp;3.0123456789&nbsp;&nbsp;&nbsp;&nbsp;0.0<br>
          &nbsp;&nbsp;16.37&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0.0&nbsp;&nbsp;2.5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0.0<br>
          &nbsp;-16.3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;0.0&nbsp;-3.012345678E-4&nbsp;-1.0<br>
          1236.3456789&nbsp;0.0&nbsp;&nbsp;7.0&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-1.2 
          </tt> <tt> </tt></td>
        <td align="left" valign="top"><tt> {<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;&nbsp;3.0&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0.0,&nbsp;-3.4&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;&nbsp;0.0},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;&nbsp;5.1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0.0,&nbsp;&nbsp;3.0123456789&nbsp;&nbsp;,&nbsp;&nbsp;0.0},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;16.37&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0.0,&nbsp;&nbsp;2.5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;&nbsp;0.0},<br>
          &nbsp;&nbsp;&nbsp;{&nbsp;-16.3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;0.0,&nbsp;-3.012345678E-4,&nbsp;-1.0},<br>
          &nbsp;&nbsp;&nbsp;{1236.3456789,&nbsp;0.0,&nbsp;&nbsp;7.0&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;,&nbsp;-1.2}<br>
          }; </tt> </td>
      </tr>
    </table>

    <p>Here are some more elaborate examples, adding labels for axes, rows, columns, 
      title and some statistical aggregations.</p>
    <table border="1" cellspacing="0">
      <tr> 
        <td nowrap> 
          <p><tt> double[,] values = {<br>
            {5 ,10, 20, 40 },<br>
            { 7, 8 , 6 , 7 },<br>
            {12 ,10, 20, 19 },<br>
            { 3, 1 , 5 , 6 }<br>
            }; <br>
            </tt><tt>string title = "CPU performance over time [nops/sec]";<br>
            string columnAxisName = "Year";<br>
            string rowAxisName = "CPU"; <br>
            string[] columnNames = {"1996", "1997", "1998", "1999"};<br>
            string[] rowNames = { "PowerBar", "Benzol", "Mercedes", "Sparcling"};<br>
            BinFunctions1D F = BinFunctions1D.functions; // alias<br>
            BinFunction1D[] aggr = {Functions.mean, Functions.rms, Functions.quantile(0.25), Functions.median, Functions.quantile(0.75), Functions.stdDev, Functions.Min, Functions.Max};<br>
            string format = "%1.2G";<br>
            DoubleMatrix2D matrix = new DenseDoubleMatrix2D(values); <br>
            new Formatter(format).toTitleString(<br>
            &nbsp;&nbsp;&nbsp;matrix,rowNames,columnNames,rowAxisName,columnAxisName,title,aggr); </tt> 
          </p>
          </td>
      </tr>
      <tr> 
        <td><tt>
    CPU&nbsp;performance&nbsp;over&nbsp;time&nbsp;[nops/sec]<br>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;Year<br>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;1996&nbsp;&nbsp;1997&nbsp;&nbsp;1998&nbsp;&nbsp;1999&nbsp;&nbsp;|&nbsp;Mean&nbsp;&nbsp;RMS&nbsp;&nbsp;&nbsp;25%&nbsp;Q.&nbsp;Median&nbsp;75%&nbsp;Q.&nbsp;StdDev&nbsp;Min&nbsp;Max<br>
    ---------------------------------------------------------------------------------------<br>
    C&nbsp;PowerBar&nbsp;&nbsp;|&nbsp;&nbsp;5&nbsp;&nbsp;&nbsp;&nbsp;10&nbsp;&nbsp;&nbsp;&nbsp;20&nbsp;&nbsp;&nbsp;&nbsp;40&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;18.75&nbsp;23.05&nbsp;&nbsp;8.75&nbsp;&nbsp;15&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;25&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;15.48&nbsp;&nbsp;&nbsp;5&nbsp;&nbsp;40&nbsp;<br>
    P&nbsp;Benzol&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;8&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;6&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;7&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;7.04&nbsp;&nbsp;6.75&nbsp;&nbsp;&nbsp;7&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;7.25&nbsp;&nbsp;&nbsp;0.82&nbsp;&nbsp;&nbsp;6&nbsp;&nbsp;&nbsp;8&nbsp;<br>
    U&nbsp;Mercedes&nbsp;&nbsp;|&nbsp;12&nbsp;&nbsp;&nbsp;&nbsp;10&nbsp;&nbsp;&nbsp;&nbsp;20&nbsp;&nbsp;&nbsp;&nbsp;19&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;15.25&nbsp;15.85&nbsp;11.5&nbsp;&nbsp;&nbsp;15.5&nbsp;&nbsp;&nbsp;19.25&nbsp;&nbsp;&nbsp;4.99&nbsp;&nbsp;10&nbsp;&nbsp;20&nbsp;<br>
    &nbsp;&nbsp;Sparcling&nbsp;|&nbsp;&nbsp;3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;6&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;3.75&nbsp;&nbsp;4.21&nbsp;&nbsp;2.5&nbsp;&nbsp;&nbsp;&nbsp;4&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;5.25&nbsp;&nbsp;&nbsp;2.22&nbsp;&nbsp;&nbsp;1&nbsp;&nbsp;&nbsp;6&nbsp;<br>
    ---------------------------------------------------------------------------------------<br>
    &nbsp;&nbsp;Mean&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;6.75&nbsp;&nbsp;7.25&nbsp;12.75&nbsp;18&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>
    &nbsp;&nbsp;RMS&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;7.53&nbsp;&nbsp;8.14&nbsp;14.67&nbsp;22.62&nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>
    &nbsp;&nbsp;25%&nbsp;Q.&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;4.5&nbsp;&nbsp;&nbsp;6.25&nbsp;&nbsp;5.75&nbsp;&nbsp;6.75&nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>
    &nbsp;&nbsp;Median&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;6&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;9&nbsp;&nbsp;&nbsp;&nbsp;13&nbsp;&nbsp;&nbsp;&nbsp;13&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>
    &nbsp;&nbsp;75%&nbsp;Q.&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;8.25&nbsp;10&nbsp;&nbsp;&nbsp;&nbsp;20&nbsp;&nbsp;&nbsp;&nbsp;24.25&nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>
    &nbsp;&nbsp;StdDev&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;3.86&nbsp;&nbsp;4.27&nbsp;&nbsp;8.38&nbsp;15.81&nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>
    &nbsp;&nbsp;Min&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;5&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;6&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>
    &nbsp;&nbsp;Max&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;12&nbsp;&nbsp;&nbsp;&nbsp;10&nbsp;&nbsp;&nbsp;&nbsp;20&nbsp;&nbsp;&nbsp;&nbsp;19&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    </tt>
    </td>
      </tr>
      <tr> 
        <td nowrap><tt> same as above, but now without aggregations<br>
          aggr=null; </tt> </td>
      </tr>
      <tr> 
        <td><tt> CPU&nbsp;performance&nbsp;over&nbsp;time&nbsp;[nops/sec]<br>
          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;Year<br>
          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;1996&nbsp;1997&nbsp;1998&nbsp;1999<br>
          ---------------------------------<br>
          C&nbsp;PowerBar&nbsp;&nbsp;|&nbsp;&nbsp;5&nbsp;&nbsp;&nbsp;10&nbsp;&nbsp;&nbsp;20&nbsp;&nbsp;&nbsp;40&nbsp;&nbsp;<br>
          P&nbsp;Benzol&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;7&nbsp;&nbsp;&nbsp;&nbsp;8&nbsp;&nbsp;&nbsp;&nbsp;6&nbsp;&nbsp;&nbsp;&nbsp;7&nbsp;&nbsp;<br>
          U&nbsp;Mercedes&nbsp;&nbsp;|&nbsp;12&nbsp;&nbsp;&nbsp;10&nbsp;&nbsp;&nbsp;20&nbsp;&nbsp;&nbsp;19&nbsp;&nbsp;<br>
          &nbsp;&nbsp;Sparcling&nbsp;|&nbsp;&nbsp;3&nbsp;&nbsp;&nbsp;&nbsp;1&nbsp;&nbsp;&nbsp;&nbsp;5&nbsp;&nbsp;&nbsp;&nbsp;6&nbsp;&nbsp; 
          </tt> </td>
      </tr>
      <tr> 
        <td nowrap>
          <p><tt> same as above, but now without rows labeled<br>
            aggr=null;<br>
            rowNames=null;<br>
            rowAxisName=null; </tt> </p>
          </td>
      </tr>
      <tr> 
        <td><tt>
    CPU&nbsp;performance&nbsp;over&nbsp;time&nbsp;[nops/sec]<br>
    Year<br>
    1996&nbsp;1997&nbsp;1998&nbsp;1999<br>
    -------------------<br>
    &nbsp;5&nbsp;&nbsp;&nbsp;10&nbsp;&nbsp;&nbsp;20&nbsp;&nbsp;&nbsp;40&nbsp;&nbsp;<br>
    &nbsp;7&nbsp;&nbsp;&nbsp;&nbsp;8&nbsp;&nbsp;&nbsp;&nbsp;6&nbsp;&nbsp;&nbsp;&nbsp;7&nbsp;&nbsp;<br>
    12&nbsp;&nbsp;&nbsp;10&nbsp;&nbsp;&nbsp;20&nbsp;&nbsp;&nbsp;19&nbsp;&nbsp;<br>
    &nbsp;3&nbsp;&nbsp;&nbsp;&nbsp;1&nbsp;&nbsp;&nbsp;&nbsp;5&nbsp;&nbsp;&nbsp;&nbsp;6&nbsp;&nbsp;
    </tt>
    </td>
      </tr>
    </table>

    <p>A column can be broader than specified by the parameter <tt>minColumnWidth</tt> 
      (because a cell may not fit into that width) but a column is never smaller than 
      <tt>minColumnWidth</tt>. Normally one does not need to specify <tt>minColumnWidth</tt> 
      (default is <tt>1</tt>). This parameter is only interesting when wanting to 
      print two distinct matrices such that both matrices have the same column width, 
      for example, to make it easier to see which column of matrix A corresponds to 
      which column of matrix B.</p>
  
    <p><b>Implementation:</b></p>

    <p>Note that this class is by no means ment to be used for high performance I/O (serialization is much quicker).
      It is ment to produce well human readable output.</p>
    <p>Analyzes the entire matrix before producing output. Each cell is converted 
      to a string as indicated by the given C-like format string. If <tt>null</tt> 
      is passed as format string, {@link java.lang.Double#ToString(double)} is used 
      instead, yielding full precision.</p>
    <p>Next, leading and trailing whitespaces are removed. For each column the maximum number of characters before 
      and after the decimal point is determined. (No problem if decimal points are 
      missing). Each cell is then padded with leading and trailing blanks, as necessary 
      to achieve decimal point aligned, left justified formatting.</p>

    @author wolfgang.hoschek@cern.ch
    @version 1.2, 11/30/99
    */

    [Serializable]
    public class FormatterDoubleAlgo : AbstractFormatter
    {
        /**
         * Constructs and returns a matrix formatter with format <tt>"%G"</tt>.
         */

        public FormatterDoubleAlgo()
            : this("%G")
        {
        }

        /**
         * Constructs and returns a matrix formatter.
         * @param format the given format used to convert a single cell value.
         */

        public FormatterDoubleAlgo(string format)
        {
            setFormat(format);
            setAlignment(DECIMAL);
        }

        /**
         * Demonstrates how to use this class.
         */

        public new static void demo1()
        {
            // parameters
            double[,] values = {
                                   {3, 0, -3.4, 0},
                                   {5.1, 0, +3.0123456789, 0},
                                   {16.37, 0.0, 2.5, 0},
                                   {-16.3, 0, -3.012345678E-4, -1},
                                   {1236.3456789, 0, 7, -1.2}
                               };
            string[] formats = {"%G", "%1.10G", "%f", "%1.2f", "%0.2e", null};


            // now the processing
            int size = formats.Length;
            DoubleMatrix2D matrix = DoubleFactory2D.dense.make(values);
            string[] strings = new string[size];
            string[] sourceCodes = new string[size];
            string[] htmlStrings = new string[size];
            string[] htmlSourceCodes = new string[size];

            for (int i = 0; i < size; i++)
            {
                string format = formats[i];
                strings[i] = new FormatterDoubleAlgo(format).ToString(matrix);
                sourceCodes[i] = new FormatterDoubleAlgo(format).toSourceCode(matrix);

                // may not compile because of packages not included in the distribution
                //htmlStrings[i] = matrixpattern.Converting.toHTML(strings[i]);
                //htmlSourceCodes[i] = matrixpattern.Converting.toHTML(sourceCodes[i]);
            }

            PrintToScreen.WriteLine("original:\n" + new FormatterDoubleAlgo().ToString(matrix));

            // may not compile because of packages not included in the distribution
            for (int i = 0; i < size; i++)
            {
                //PrintToScreen.WriteLine("\nhtmlString("+formats[i]+"):\n"+htmlStrings[i]);
                //PrintToScreen.WriteLine("\nhtmlSourceCode("+formats[i]+"):\n"+htmlSourceCodes[i]);
            }

            for (int i = 0; i < size; i++)
            {
                PrintToScreen.WriteLine("\nstring(" + formats[i] + "):\n" + strings[i]);
                PrintToScreen.WriteLine("\nsourceCode(" + formats[i] + "):\n" + sourceCodes[i]);
            }
        }

        /**
         * Demonstrates how to use this class.
         */

        public new static void demo2()
        {
            // parameters
            double[] values = {
                                  //5, 0.0, -0.0, -double.NaN, double.NaN, 0.0/0.0, Double.NegativeInfinity, Double.PositiveInfinity, Double.MinValue, Double.MaxValue
                                  5, 0.0, -0.0, -double.NaN, double.NaN, 0.0/0.0, Double.MinValue, Double.MaxValue,
                                  Double.NegativeInfinity, Double.PositiveInfinity
                                  //Double.MinValue, Double.MaxValue //, Double.NegativeInfinity, Double.PositiveInfinity
                              };
            //string[] formats =         {"%G", "%1.10G", "%f", "%1.2f", "%0.2e"};
            string[] formats = {"%G", "%1.19G"};


            // now the processing
            int size = formats.Length;
            DoubleMatrix1D matrix = new DenseDoubleMatrix1D(values);

            string[] strings = new string[size];
            //string[] javaStrings = new string[size];

            for (int i = 0; i < size; i++)
            {
                string format = formats[i];
                strings[i] = new FormatterDoubleAlgo(format).ToString(matrix);
                for (int j = 0; j < matrix.Size(); j++)
                {
                    PrintToScreen.WriteLine((matrix.get(j)));
                }
            }

            PrintToScreen.WriteLine("original:\n" + new FormatterDoubleAlgo().ToString(matrix));

            for (int i = 0; i < size; i++)
            {
                PrintToScreen.WriteLine("\nstring(" + formats[i] + "):\n" + strings[i]);
            }
        }

        /**
         * Demonstrates how to use this class.
         */

        public static void demo3(int size, double value)
        {
            Timer timer = new Timer();
            string s;
            StringBuilder buf;
            DoubleMatrix2D matrix = DoubleFactory2D.dense.make(size, size, value);

            timer.reset().start();
            buf = new StringBuilder();
            for (int i = size; --i >= 0;)
            {
                for (int j = size; --j >= 0;)
                {
                    buf.Append(matrix.getQuick(i, j));
                }
            }
            buf = null;
            timer.stop().display();

            timer.reset().start();
            Former format = new FormerFactory().create("%G");
            buf = new StringBuilder();
            for (int i = size; --i >= 0;)
            {
                for (int j = size; --j >= 0;)
                {
                    buf.Append(format.form(matrix.getQuick(i, j)));
                }
            }
            buf = null;
            timer.stop().display();

            timer.reset().start();
            s = new FormatterDoubleAlgo(null).ToString(matrix);
            //PrintToScreen.WriteLine(s);
            s = null;
            timer.stop().display();

            timer.reset().start();
            s = new FormatterDoubleAlgo("%G").ToString(matrix);
            //PrintToScreen.WriteLine(s);
            s = null;
            timer.stop().display();
        }

        /**
         * Demonstrates how to use this class.
         */

        public static void demo4()
        {
            // parameters
            double[,] values = {
                                   {3, 0, -3.4, 0},
                                   {5.1, 0, +3.0123456789, 0},
                                   {16.37, 0.0, 2.5, 0},
                                   {-16.3, 0, -3.012345678E-4, -1},
                                   {1236.3456789, 0, 7, -1.2}
                               };
            /*
            double[,] values = {
                {3,     1,      },
                {5.1   ,16.37,  }
            };
            */
            //string[] columnNames = { "he",   "",  "he", "four" };
            //string[] rowNames = { "hello", "du", null, "abcdef", "five" };
            string[] columnNames = {"0.1", "0.3", "0.5", "0.7"};
            string[] rowNames = {"SunJDK1.2.2 classic", "IBMJDK1.1.8", "SunJDK1.3 Hotspot", "other1", "other2"};
            //string[] columnNames = { "0.1", "0.3" };
            //string[] rowNames = { "SunJDK1.2.2 classic", "IBMJDK1.1.8"};

            DoubleMatrix2D matrix = DoubleFactory2D.dense.make(values);
            PrintToScreen.WriteLine("\n\n" +
                              new FormatterDoubleAlgo("%G").toTitleString(matrix, rowNames, columnNames, "rowAxis",
                                                                          "colAxis",
                                                                          "VM Performance: Provider vs. matrix density"));
        }

        /**
         * Demonstrates how to use this class.
         */

        public static void demo5()
        {
            // parameters
            double[,] values = {
                                   {3, 0, -3.4, 0},
                                   {5.1, 0, +3.0123456789, 0},
                                   {16.37, 0.0, 2.5, 0},
                                   {-16.3, 0, -3.012345678E-4, -1},
                                   {1236.3456789, 0, 7, -1.2}
                               };
            /*
            double[,] values = {
                {3,     1,      },
                {5.1   ,16.37,  }
            };
            */
            //string[] columnNames = { "he",   "",  "he", "four" };
            //string[] rowNames = { "hello", "du", null, "abcdef", "five" };
            string[] columnNames = {"0.1", "0.3", "0.5", "0.7"};
            string[] rowNames = {"SunJDK1.2.2 classic", "IBMJDK1.1.8", "SunJDK1.3 Hotspot", "other1", "other2"};
            //string[] columnNames = { "0.1", "0.3" };
            //string[] rowNames = { "SunJDK1.2.2 classic", "IBMJDK1.1.8"};

            PrintToScreen.WriteLine(DoubleFactory2D.dense.make(values));
            PrintToScreen.WriteLine(new FormatterDoubleAlgo("%G").toTitleString(DoubleFactory2D.dense.make(values), rowNames,
                                                                          columnNames, "vendor", "density", "title"));
        }

        /**
         * Demonstrates how to use this class.
         */

        public static void demo6()
        {
            // parameters
            double[,] values = {
                                   {3, 0, -3.4, 0},
                                   {5.1, 0, +3.0123456789, 0},
                                   {16.37, 0.0, 2.5, 0},
                                   {-16.3, 0, -3.012345678E-4, -1},
                                   {1236.3456789, 0, 7, -1.2}
                               };
            /*
            double[,] values = {
                {3,     1,      },
                {5.1   ,16.37,  }
            };
            */
            //string[] columnNames = { "he",   "",  "he", "four" };
            //string[] rowNames = { "hello", "du", null, "abcdef", "five" };
            //string[] columnNames = { "0.1", "0.3", "0.5", "0.7" };
            string[] columnNames = {"W", "X", "Y", "Z"};
            string[] rowNames = {"SunJDK1.2.2 classic", "IBMJDK1.1.8", "SunJDK1.3 Hotspot", "other1", "other2"};
            //string[] columnNames = { "0.1", "0.3" };
            //string[] rowNames = { "SunJDK1.2.2 classic", "IBMJDK1.1.8"};

            //PrintToScreen.WriteLine(DoubleFactory2D.dense.make(values)); 
            //PrintToScreen.WriteLine(new Formatter().toSourceCode(DoubleFactory2D.dense.make(values)));
            PrintToScreen.WriteLine(new FormatterDoubleAlgo().ToString(DoubleFactory2D.dense.make(values)));
            PrintToScreen.WriteLine(new FormatterDoubleAlgo().toTitleString(DoubleFactory2D.dense.make(values), rowNames,
                                                                      columnNames, "vendor", "density", "title"));
        }

        /**
         * Demonstrates how to use this class.
         */

        public static void demo7()
        {
            // parameters
            /*
            double[,] values = {
                {3,     0,        -3.4, 0},
                {5.1   ,0,        +3.0123456789, 0},
                {16.37, 0.0,       2.5, 0},
                {-16.3, 0,        -3.012345678E-4, -1},
                {1236.3456789, 0,  7, -1.2}
            };
            */
            double[,] values = {
                                   {5, 10, 20, 40},
                                   {7, 8, 6, 7},
                                   {12, 10, 20, 19},
                                   {3, 1, 5, 6}
                               };
            string[] columnNames = {"1996", "1997", "1998", "1999"};
            string[] rowNames = {"PowerBar", "Benzol", "Mercedes", "Sparcling"};
            string rowAxisName = "CPU";
            string columnAxisName = "Year";
            string title = "CPU performance over time [nops/sec]";
            BinFunctions1D F = BinFunctions1D.functions;
            BinFunction1D[] aggr =
                {
                    BinFunctions1D.mean,
                    BinFunctions1D.rms,
                    BinFunctions1D.quantile(0.25),
                    BinFunctions1D.median,
                    BinFunctions1D.quantile(0.75),
                    BinFunctions1D.stdDev,
                    BinFunctions1D.min,
                    BinFunctions1D.max
                };

            string format = "%1.2G";

            //string[] columnNames = { "W", "X", "Y", "Z", "mean", "median", "sum"};
            //string[] rowNames = { "SunJDK1.2.2 classic", "IBMJDK1.1.8", "SunJDK1.3 Hotspot", "other1", "other2", "mean", "median", "sum" };
            //BinFunction1D[] aggr = {Functions.mean, Functions.median, Functions.sum};

            //PrintToScreen.WriteLine(DoubleFactory2D.dense.make(values)); 
            //PrintToScreen.WriteLine(new Formatter().toSourceCode(DoubleFactory2D.dense.make(values)));
            //PrintToScreen.WriteLine(new Formatter().ToString(DoubleFactory2D.dense.make(values)));
            //PrintToScreen.WriteLine(new Formatter().toTitleString(DoubleFactory2D.dense.make(values),rowNames,columnNames,rowAxisName,columnAxisName,title));
            PrintToScreen.WriteLine(new FormatterDoubleAlgo(format).toTitleString(DoubleFactory2D.dense.make(values), rowNames,
                                                                            columnNames, rowAxisName, columnAxisName,
                                                                            title, aggr));
            //PrintToScreen.WriteLine(matrixpattern.Converting.toHTML(new Formatter(format).toTitleString(DoubleFactory2D.dense.make(values),rowNames,columnNames,rowAxisName,columnAxisName,title, aggr)));
        }

        /**
         * Converts a given cell to a string; no alignment considered.
         */

        public string form(DoubleMatrix1D matrix, int index, Former formatter)
        {
            return formatter.form(matrix.get(index));
        }

        /**
         * Converts a given cell to a string; no alignment considered.
         */

        public override string form(AbstractMatrix1D matrix, int index, Former formatter)
        {
            return form((DoubleMatrix1D) matrix, index, formatter);
        }

        /**
         * Returns a string representations of all cells; no alignment considered.
         */

        public string[,] format(DoubleMatrix2D matrix)
        {
            string[,] strings = new string[matrix.Rows(),matrix.Columns()];
            for (int row = matrix.Rows(); --row >= 0;)
            {
                ArrayHelper.SetRow(
                    strings,
                    formatRow(matrix.viewRow(row)),
                    row);
            }
            return strings;
        }

        /**
         * Returns a string representations of all cells; no alignment considered.
         */

        public override string[,] format(AbstractMatrix2D matrix)
        {
            return format((DoubleMatrix2D) matrix);
        }

        /**
         * Returns the index of the decimal point.
         */

        public int indexOfDecimalPoint(string s)
        {
            int i = s.LastIndexOf('.');
            if (i < 0)
            {
                i = s.LastIndexOf('e');
            }
            if (i < 0)
            {
                i = s.LastIndexOf('E');
            }
            if (i < 0)
            {
                i = s.Length;
            }
            return i;
        }

        /**
         * Returns the number of characters before the decimal point.
         */

        public new int lead(string s)
        {
            if (m_alignment.Equals(DECIMAL))
            {
                return indexOfDecimalPoint(s);
            }
            return base.lead(s);
        }

        /**
         * Returns a string <tt>s</tt> such that <tt>Object[] m = s</tt> is a legal Java statement.
         * @param matrix the matrix to format.
         */

        public string toSourceCode(DoubleMatrix1D matrix)
        {
            FormatterDoubleAlgo copy = (FormatterDoubleAlgo) Clone();
            copy.setPrintShape(false);
            copy.setColumnSeparator(", ");
            string lead = "{";
            string trail = "};";
            return lead + copy.ToString(matrix) + trail;
        }

        /**
         * Returns a string <tt>s</tt> such that <tt>Object[] m = s</tt> is a legal Java statement.
         * @param matrix the matrix to format.
         */

        public string toSourceCode(DoubleMatrix2D matrix)
        {
            FormatterDoubleAlgo copy = (FormatterDoubleAlgo) Clone();
            string b3 = blanks(3);
            copy.setPrintShape(false);
            copy.setColumnSeparator(", ");
            copy.setRowSeparator("},\n" + b3 + "{");
            string lead = "{\n" + b3 + "{";
            string trail = "}\n};";
            return lead + copy.ToString(matrix) + trail;
        }

        /**
         * Returns a string <tt>s</tt> such that <tt>Object[] m = s</tt> is a legal Java statement.
         * @param matrix the matrix to format.
         */

        public string toSourceCode(DoubleMatrix3D matrix)
        {
            FormatterDoubleAlgo copy = (FormatterDoubleAlgo) Clone();
            string b3 = blanks(3);
            string b6 = blanks(6);
            copy.setPrintShape(false);
            copy.setColumnSeparator(", ");
            copy.setRowSeparator("},\n" + b6 + "{");
            copy.setSliceSeparator("}\n" + b3 + "},\n" + b3 + "{\n" + b6 + "{");
            string lead = "{\n" + b3 + "{\n" + b6 + "{";
            string trail = "}\n" + b3 + "}\n}";
            return lead + copy.ToString(matrix) + trail;
        }

        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */

        public string ToString(DoubleMatrix1D matrix)
        {
            DoubleMatrix2D easy = matrix.like2D(1, matrix.Size());
            easy.viewRow(0).assign(matrix);
            return ToString(easy);
        }

        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */

        public string ToString(DoubleMatrix2D matrix)
        {
            return base.ToString(matrix);
        }

        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */

        public string ToString(DoubleMatrix3D matrix)
        {
            StringBuilder buf = new StringBuilder();
            bool oldPrintShape = m_printShape;
            m_printShape = false;
            for (int slice = 0; slice < matrix.Slices(); slice++)
            {
                if (slice != 0)
                {
                    buf.Append(m_sliceSeparator);
                }
                buf.Append(ToString(matrix.viewSlice(slice)));
            }
            m_printShape = oldPrintShape;
            if (m_printShape)
            {
                buf.Insert(0, shape(matrix) + Environment.NewLine);
            }
            return buf.ToString();
        }

        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */

        public new string ToString(AbstractMatrix2D matrix)
        {
            return ToString((DoubleMatrix2D) matrix);
        }

        /**
        Returns a string representation of the given matrix with axis as well as rows and columns labeled.
        Pass <tt>null</tt> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.

        @param matrix The matrix to format.
        @param rowNames The headers of all rows (to be put to the left of the matrix).
        @param columnNames The headers of all columns (to be put to above the matrix).
        @param rowAxisName The label of the y-axis.
        @param columnAxisName The label of the x-axis.
        @param title The overall title of the matrix to be formatted.
        @return the matrix converted to a string.
        */

        public string toTitleString(DoubleMatrix2D matrix, string[] rowNames, string[] columnNames, string rowAxisName,
                                    string columnAxisName, string title)
        {
            if (matrix.Size() == 0)
            {
                return "Empty matrix";
            }
            string[,] s = format(matrix);
            //string oldAlignment = alignment;
            //alignment = DECIMAL;
            align(s);
            //alignment = oldAlignment;
            return new FormatterObjectAlgo().toTitleString(
                ObjectFactory2D.dense.make(s),
                rowNames,
                columnNames,
                rowAxisName,
                columnAxisName,
                title);
        }

        /**
        Same as <tt>toTitleString</tt> except that additionally statistical aggregates (mean, median, sum, etc.) of rows and columns are printed.
        Pass <tt>null</tt> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.

        @param matrix The matrix to format.
        @param rowNames The headers of all rows (to be put to the left of the matrix).
        @param columnNames The headers of all columns (to be put to above the matrix).
        @param rowAxisName The label of the y-axis.
        @param columnAxisName The label of the x-axis.
        @param title The overall title of the matrix to be formatted.
        @param aggr the aggregation functions to be applied to columns and rows.
        @return the matrix converted to a string.
        @see BinFunction1D
        @see BinFunctions1D
        */

        public string toTitleString(
            DoubleMatrix2D matrix,
            string[] rowNames,
            string[] columnNames,
            string rowAxisName,
            string columnAxisName,
            string title,
            BinFunction1D[] aggr)
        {
            if (matrix.Size() == 0)
            {
                return "Empty matrix";
            }
            if (aggr == null || aggr.Length == 0)
            {
                return toTitleString(matrix, rowNames, columnNames, rowAxisName, columnAxisName, title);
            }

            DoubleMatrix2D rowStats = matrix.like(matrix.Rows(), aggr.Length); // hold row aggregations
            DoubleMatrix2D colStats = matrix.like(aggr.Length, matrix.Columns()); // hold column aggregations

            Statistic.aggregate(matrix, aggr, colStats); // aggregate an entire column at a time
            Statistic.aggregate(matrix.viewDice(), aggr, rowStats.viewDice()); // aggregate an entire row at a time

            // turn into strings
            // tmp holds "matrix" plus "colStats" below (needed so that numbers in a columns can be decimal point aligned)
            DoubleMatrix2D tmp = matrix.like(matrix.Rows() + aggr.Length, matrix.Columns());
            tmp.viewPart(0, 0, matrix.Rows(), matrix.Columns()).assign(matrix);
            tmp.viewPart(matrix.Rows(), 0, aggr.Length, matrix.Columns()).assign(colStats);
            colStats = null;

            string[,] s1 = format(tmp);
            align(s1);
            tmp = null;
            string[,] s2 = format(rowStats);
            align(s2);
            rowStats = null;

            // copy strings into a large matrix holding the source matrix and all aggregations
            ObjectMatrix2D allStats = ObjectFactory2D.dense.make(matrix.Rows() + aggr.Length,
                                                                 matrix.Columns() + aggr.Length + 1);
            allStats.viewPart(0, 0, matrix.Rows() + aggr.Length, matrix.Columns()).assign(s1);
            allStats.viewColumn(matrix.Columns()).assign("|");
            allStats.viewPart(0, matrix.Columns() + 1, matrix.Rows(), aggr.Length).assign(s2);
            s1 = null;
            s2 = null;

            // Append a vertical "|" separator plus names of aggregation functions to line holding columnNames
            if (columnNames != null)
            {
                ObjectArrayList list = new ObjectArrayList(columnNames);
                list.Add("|");
                for (int i = 0; i < aggr.Length; i++)
                {
                    list.Add(aggr[i].name()); // Add names of aggregation functions
                }
                columnNames = new string[list.Size()];
                list.ToArray(columnNames);
            }

            // Append names of aggregation functions to line holding rowNames
            if (rowNames != null)
            {
                ObjectArrayList list = new ObjectArrayList(rowNames);
                for (int i = 0; i < aggr.Length; i++)
                {
                    list.Add(aggr[i].name()); // Add names of aggregation functions
                }
                rowNames = new string[list.Size()];
                list.ToArray(rowNames);
            }

            // turn large matrix into string
            string s = new FormatterObjectAlgo().toTitleString(
                allStats,
                rowNames,
                columnNames,
                rowAxisName,
                columnAxisName,
                title);

            // Insert a horizontal "----------------------" separation line above the column stats
            // determine insertion position and line width
            int last = s.Length + 1;
            int secondLast = last;
            int v = Math.Max(0, rowAxisName == null ? 0 : rowAxisName.Length - matrix.Rows() - aggr.Length);
            for (int k = 0; k < aggr.Length + 1 + v; k++)
            {
                // scan "aggr.Length+1+v" lines backwards
                secondLast = last;
                last = s.LastIndexOf(m_rowSeparator, last - 1);
            }
            StringBuilder buf = new StringBuilder(s);
            buf.Insert(secondLast, m_rowSeparator + repeat('-', secondLast - last - 1));

            return buf.ToString();
        }

        /**
        Returns a string representation of the given matrix with axis as well as rows and columns labeled.
        Pass <tt>null</tt> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.

        @param matrix The matrix to format.
        @param sliceNames The headers of all slices (to be put above each slice).
        @param rowNames The headers of all rows (to be put to the left of the matrix).
        @param columnNames The headers of all columns (to be put to above the matrix).
        @param sliceAxisName The label of the z-axis (to be put above each slice).
        @param rowAxisName The label of the y-axis.
        @param columnAxisName The label of the x-axis.
        @param title The overall title of the matrix to be formatted.
        @param aggr the aggregation functions to be applied to columns, rows.
        @return the matrix converted to a string.
        @see BinFunction1D
        @see BinFunctions1D
        */

        public string toTitleString(DoubleMatrix3D matrix, string[] sliceNames, string[] rowNames, string[] columnNames,
                                    string sliceAxisName, string rowAxisName, string columnAxisName, string title,
                                    BinFunction1D[] aggr)
        {
            if (matrix.Size() == 0)
            {
                return "Empty matrix";
            }
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < matrix.Slices(); i++)
            {
                if (i != 0)
                {
                    buf.Append(m_sliceSeparator);
                }
                buf.Append(toTitleString(matrix.viewSlice(i), rowNames, columnNames, rowAxisName, columnAxisName,
                                         title + Environment.NewLine + sliceAxisName + "=" + sliceNames[i], aggr));
            }
            return buf.ToString();
        }

        /**
        Returns a string representation of the given matrix with axis as well as rows and columns labeled.
        Pass <tt>null</tt> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.

        @param matrix The matrix to format.
        @param sliceNames The headers of all slices (to be put above each slice).
        @param rowNames The headers of all rows (to be put to the left of the matrix).
        @param columnNames The headers of all columns (to be put to above the matrix).
        @param sliceAxisName The label of the z-axis (to be put above each slice).
        @param rowAxisName The label of the y-axis.
        @param columnAxisName The label of the x-axis.
        @param title The overall title of the matrix to be formatted.
        @return the matrix converted to a string.
        */

        private string xtoTitleString(DoubleMatrix3D matrix, string[] sliceNames, string[] rowNames,
                                      string[] columnNames, string sliceAxisName, string rowAxisName,
                                      string columnAxisName, string title)
        {
            if (matrix.Size() == 0)
            {
                return "Empty matrix";
            }
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < matrix.Slices(); i++)
            {
                if (i != 0)
                {
                    buf.Append(m_sliceSeparator);
                }
                buf.Append(toTitleString(matrix.viewSlice(i), rowNames, columnNames, rowAxisName, columnAxisName,
                                         title + Environment.NewLine + sliceAxisName + "=" + sliceNames[i]));
            }
            return buf.ToString();
        }
    }
}
