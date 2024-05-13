import logo from './doingazure-logo.svg';
import './App.css';
import { useState } from 'react';

function App() {
  const [domain, setDomain] = useState('');
  const [ssldays, setSsldays] = useState('');

  var computedSsldays = 42;

  const handleButtonClick = () => {
    const url = `/api/ssldays?name=${encodeURIComponent(domain)}`;
    alert(`ssldays API: ${url}`);
    fetch(url)
      .then(response => response.json())
      .then(data => {
        computedSsldays = data.sslDays;
        setSsldays(data.sslDays);
        console.log(`ssldays API results: ${data.sslDays} days remain on SSL certificate for ${domain}`);
      })
      .catch(error => console.error(error));

      console.log(`ssldays API results: --- ${ssldays} days (computed: ${computedSsldays} days) remain on SSL certificate for ${domain}`);
  };

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />      
      </header>
      <main>
        <input type="text" placeholder="Enter domain name" value={domain} onChange={e => setDomain(e.target.value)} />
        <button onClick={handleButtonClick}>Get SSL Days</button>
        <p>{ssldays} (full) days remain on SSL certificate for {domain}</p>
      </main>
      <footer>
        <small>Copyright (c) 2024, Doing Azure, TBD but prob will be licensed under CC</small>
      </footer>
    </div>
  );
}


export default App;
