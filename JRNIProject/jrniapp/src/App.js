import logo from './logo.svg';
import './App.css';
import { useState } from 'react';


function App() {
  const [data, setData] = useState("");
  const [dataobj,setObjData]=useState([]);
  const [hasresponse,setResponse]=useState(false);
  const  callGet=(data)=>{
   
    console.log(data);
   fetch("https://localhost:7250/CalendarEvents?email="+data)
   .then(res => res.json())
   .then(
     (result) => {
       console.log(result);
    
      
       setObjData(result);
       setResponse(true);
      return result.json;
     },
     
     (error) => {
       
     }
   )
  }
  const onChangeHandler = event => {
    setData(event.target.value);
  };
  
  return (
    <div className="App">
      <header className="App-header">
        <h1>Please Enter your Email</h1>
     <input onChange={onChangeHandler} value={data}></input>
     <button onClick={()=>callGet(data)}>get data</button>
    <table>
     
     <thead>
     <tr>
     <th>Email     </th>
     <th>Busy Event Count</th>
     <th>OOO Event Count</th>
     </tr>
     </thead>
     <tbody>
     <tr>
       <td>{dataobj.emailId}</td>
       <td> {dataobj.busyCount}</td>
       <td> {dataobj.outOfOfficeCount}</td>
     </tr>
    
    
  </tbody>
   </table>
   <table>
     
     <thead>
     <tr>
     <th>Event Summary     </th>
     <th>Start Time</th>
     <th>End Time</th>
     </tr>
     </thead>
     <tbody>
   
     
         
    {hasresponse && dataobj.events.map(function(item, i){
  console.log(item.summary);
  return <tr>
    <td>{item.summary}</td>
    <td>{item.startTime}</td>
    <td>{item.endTime}</td>
    </tr>;
}

    )
}
         
    
  </tbody>
   </table>
   
      </header>
    </div>
  );
}



export default App;
