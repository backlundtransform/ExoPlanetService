import * as React from 'react'
import {useState, useEffect,useRef} from 'react'

  import * as am4core from "@amcharts/amcharts4/core";
import * as am4charts from "@amcharts/amcharts4/charts";
import am4themes_animated from "@amcharts/amcharts4/themes/animated";

import {GetTransitAsync  } from '../service/getMast'

const TimeSeriesChart=({ra,dec,radius}:{ra:number,  dec:number, radius:number}): JSX.Element=> {

  const chart = useRef<null | am4charts.XYChart>(null)
  const [data, setData]= useState([])
  useEffect(()=>{
  GetTransitAsync(ra,dec, radius).then(setData)
  
  },[])
 useEffect(()=>{
 
    am4core.useTheme(am4themes_animated);

  
    let chart = am4core.create("timeseriesdiv", am4charts.XYChart);
      
    chart.data = [];
    
    let dateAxis = chart.xAxes.push(new am4charts.CategoryAxis());
    dateAxis.renderer.minGridDistance = 60;
  
    let series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.valueY = "value";
    series.dataFields.categoryX = "index";
    series.tooltipText = "{value}"
    
    series.tooltip.pointerOrientation = "vertical";
    
    chart.cursor = new am4charts.XYCursor();
    chart.cursor.snapToSeries = series;
    chart.cursor.xAxis = dateAxis;
  
 },[])

 useEffect(()=>{
 
  if (chart && chart.current) {
    chart.current.data = data
}

},[data])

return <div id ={'timeseriesdiv'}   style={{
    width: '100%',
    maxHeight: '700px',
    height: '69vh',
    margin: '10px'
  }}></div>
}

export default TimeSeriesChart