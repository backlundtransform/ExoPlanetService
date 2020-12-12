import * as React from 'react'
import {useState,   useLayoutEffect,useRef} from 'react'

  import * as am4core from "@amcharts/amcharts4/core";
import * as am4charts from "@amcharts/amcharts4/charts";
import am4themes_animated from "@amcharts/amcharts4/themes/animated";

import { GetTransitAsync  } from '../service/getMast'

const groupBy = (items:TransitData[], key:string) => items.reduce(
  (result, item) => ({
    ...result,
    [item[key]]: [
      ...(result[item[key]] || []),
      item,
    ],
  }), 
  {},
);

interface TransitData { index:number, value:number, label:string}

const TimeSeriesChart=({ra,dec,radius}:{ra:number,  dec:number, radius:number}): JSX.Element=> {

  const chart = useRef<null | am4charts.XYChart>(null)
  const [data, setData]= useState<TransitData[]>([])
  useLayoutEffect(()=>{

     GetTransitAsync(ra,dec, radius).then(setData)
  
  },[])
  useLayoutEffect(()=>{
 
    am4core.useTheme(am4themes_animated)
    let x= am4core.create("timeseriesdiv", am4charts.XYChart)     
    x.data = [] 

    let  categoryAxis = x.xAxes.push(new am4charts.CategoryAxis())
    categoryAxis.renderer.minGridDistance = 60
    categoryAxis.dataFields.category = 'index'
    let valueAxis = x.yAxes.push(new am4charts.ValueAxis())
    valueAxis.dataFields.data = 'value'
    x.cursor = new am4charts.XYCursor()
    x.cursor.xAxis =categoryAxis 
    chart.current =x
    return () => {
      x.dispose()
    }
  
 },[])

 useLayoutEffect(()=>{
 
  if (chart && chart.current) {
   let groupedData = groupBy(data, "label")


 for(const item of Object.keys(groupedData)){
   let series = chart.current.series.push(new am4charts.LineSeries())
    series.dataFields.valueY = item
    series.dataFields.categoryX = 'index'
    series.tooltipText =item 
    series.tooltip.pointerOrientation = 'vertical'
}

    chart.current.data = data.map(p=>{
      return { [p.label]: p.value, index:p.index}
    })
}

},[data])

return <div id ={'timeseriesdiv'}  style={{
    width: '100%',
    maxHeight: '700px',
    height: '69vh',
    margin: '10px'
  }}></div>
}

export default TimeSeriesChart