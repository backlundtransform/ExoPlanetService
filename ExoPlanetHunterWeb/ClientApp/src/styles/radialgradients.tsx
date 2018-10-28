import  * as React from "react";

import{resource} from '../config/Resource'

import {
    Defs,
  Stop,
  RadialGradient,
} from 'react-native-svg-web';
import { Star } from "../service/getPlanets";


export const Gradient =(star:Star)=>(<Defs><RadialGradient id={"jovian"}
  gradientUnits="objectBoundingBox" fx="30%" fy="30%">
  <Stop offset="0%" stopColor="#FFFFFF" />
  <Stop offset="30%" stopColor="#f6e9e0" />
  <Stop offset="100%" stopColor="#600000" />
</RadialGradient>
<RadialGradient id={"iron"}
  gradientUnits="objectBoundingBox" fx="30%" fy="30%">
  <Stop offset="0%" stopColor="#FFFFFF"  />
  <Stop offset="30%" stopColor="#dddcdb" />
  <Stop offset="100%" stopColor="#c9d3d8" />
</RadialGradient>
<RadialGradient id={"hotsuperearth"}
  gradientUnits="objectBoundingBox" fx="30%" fy="30%">

  <Stop offset="30%" stopColor="#ffcc7f" />
  <Stop offset="100%" stopColor="#fc6b11"/>
</RadialGradient>
<RadialGradient id={"hotjupiter"}
  gradientUnits="objectBoundingBox" fx="30%" fy="30%">
  <Stop offset="0%" stopColor="#FFFFFF" />
  <Stop offset="30%" stopColor="#f4af0e" />
  <Stop offset="100%" stopColor="#f41d1d" />
</RadialGradient>
<RadialGradient id={"hotstone"}
  gradientUnits="objectBoundingBox" fx="30%" fy="30%">

  <Stop offset="30%" stopColor="#ffcc7f" />
  <Stop offset="100%" stopColor="#fc6b11"/>
</RadialGradient>
<RadialGradient id={"stone"}
  gradientUnits="objectBoundingBox" fx="30%" fy="30%">
  <Stop offset="0%" stopColor="#4286f4" />
  <Stop offset="80%" stopColor="#4286f4" />
  <Stop offset="100%" stopColor="#2baf30"/>
</RadialGradient>
<RadialGradient id={"coldstone"}
  gradientUnits="objectBoundingBox" fx="30%" fy="30%">
  <Stop offset="30%" stopColor="#f2f2f7"/>
  <Stop offset="100%" stopColor="#8bb1ed" />

</RadialGradient>
<RadialGradient id={"neptunian"}
  gradientUnits="objectBoundingBox" fx="30%" fy="30%">
  <Stop offset="0%" stopColor="#b0b0e8" />
  <Stop offset="30%" stopColor="#9595e2" />
  <Stop offset="100%" stopColor="#a5a5d1" />
</RadialGradient>
<RadialGradient id={"superearth"}
  gradientUnits="objectBoundingBox" fx="30%" fy="30%">

  <Stop offset="30%" stopColor="#4286f4" />
  <Stop offset="100%" stopColor="#4286f4"/>
   </RadialGradient>
  <RadialGradient id={"coldsuperearth"}
  gradientUnits="objectBoundingBox" fx="30%" fy="30%">
  <Stop offset="30%" stopColor="#f2f2f7"/>
  <Stop offset="100%" stopColor="#8bb1ed" />
 
</RadialGradient>< RadialGradient id={`Star-${resource.color[3]}`}   cx={ star.radius} cy={ 0.1*star.radius} r={ star.radius} fx="25" fy="25">
<Stop offset="10%" stopColor="#ff6a00"/>
 <Stop offset="70%" stopColor="red"/>
 <Stop offset="100%" stopColor="black" stopOpacity={0}/>     
</ RadialGradient>
< RadialGradient id={`Startop-${resource.color[3]}`}   cx={ star.radius} cy={ 1.1*star.radius} r={ star.radius} fx="30%" fy="30%">
<Stop offset="10%" stopColor="#ff6a00"/>
 <Stop offset="70%" stopColor="red"/>
 <Stop offset="100%" stopColor="black" stopOpacity={0}/>      
</ RadialGradient>
< RadialGradient id={`Star-${resource.color[2]}`}  cx={ star.radius} cy={ 0.1*star.radius} r={ star.radius} fx="30%" fy="30%">
<Stop offset="10%" stopColor="#ff6a00"/>
 <Stop offset="70%" stopColor="orange"/>
 <Stop offset="100%" stopColor="black" stopOpacity={0}/>    
</ RadialGradient>
< RadialGradient id={`Startop-${resource.color[2]}`}   cx={ star.radius} cy={ 1.1*star.radius} r={ star.radius} fx="30%" fy="30%">
 <Stop offset="10%" stopColor="#ff6a00"/>
 <Stop offset="70%" stopColor="orange"/>
 <Stop offset="100%" stopColor="black" stopOpacity={0}/>     
</ RadialGradient>
< RadialGradient id={`Star-${resource.color[1]}`}  cx={ star.radius} cy={ 0.1*star.radius} r={ star.radius} fx="30%" fy="30%">
<Stop offset="10%" stopColor="#fffdd6"/>
 <Stop offset="70%" stopColor="white"/>
 <Stop offset="100%" stopColor="black" stopOpacity={0}/>
</ RadialGradient>
< RadialGradient id={`Startop-${resource.color[1]}`}   cx={ star.radius} cy={ 1.1*star.radius} r={ star.radius} fx="30%" fy="30%">
<Stop offset="10%" stopColor="#fffdd6"/>
 <Stop offset="70%" stopColor="white"/>
 <Stop offset="100%" stopColor="black" stopOpacity={0}/> 
</ RadialGradient>
< RadialGradient id={`Star-${resource.color[0]}`}   cx={ star.radius} cy={ 0.1*star.radius} r={ star.radius} fx="30%" fy="30%">
<Stop offset="10%" stopColor="#164dff"/>
 <Stop offset="70%" stopColor="blue"/>
 <Stop offset="100%" stopColor="black" stopOpacity={0}/> 
</ RadialGradient>
< RadialGradient id={`Startop-${resource.color[0]}`}   cx={ star.radius} cy={ 1.1*star.radius} r={ star.radius} fx="30%" fy="30%">
 <Stop offset="10%" stopColor="#164dff"/>
 <Stop offset="70%" stopColor="blue"/>
 <Stop offset="100%" stopColor="black" stopOpacity={0}/>  
</ RadialGradient>

< RadialGradient id={`${resource.color[3]}`} gradientUnits="objectBoundingBox" fx="30%" fy="30%">
 <Stop offset="10%" stopColor="#ff6a00"/>
 <Stop offset="50%" stopColor="red"/>
 <Stop offset="80%" stopColor="#50607a"/>   
</ RadialGradient>
< RadialGradient id={`${resource.color[2]}`}    gradientUnits="objectBoundingBox" fx="30%" fy="30%">
<Stop offset="10%" stopColor="#ff6a00"/>
 <Stop offset="50%" stopColor="orange"/>
 <Stop offset="80%" stopColor="#50607a"/>
</ RadialGradient>
< RadialGradient id={`${resource.color[1]}`}    gradientUnits="objectBoundingBox" fx="30%" fy="30%">
<Stop offset="10%" stopColor="#fffdd6"/>
 <Stop offset="50%" stopColor="white"/>
 <Stop offset="80%" stopColor="#50607a"/>
</ RadialGradient>
< RadialGradient id={`${resource.color[0]}`}    gradientUnits="objectBoundingBox" fx="30%" fy="30%">
<Stop offset="10%" stopColor="#164dff"/>
 <Stop offset="50%" stopColor="blue"/>
 <Stop offset="80%" stopColor="#50607a"/>
</ RadialGradient>
 	</Defs>)
    
