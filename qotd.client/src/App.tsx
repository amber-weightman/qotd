import { useEffect, useState } from 'react';
import './App.css';
import WeatherTable from './components/WeatherTable';
import Forecast from './interfaces/forecast';
import LoadingMessage from './components/LoadingMessage';

function App() {
  const [forecasts, setForecasts] = useState<Forecast[]>();

  useEffect(() => {
    populateWeatherData();
  }, []);

  const contents = forecasts === undefined ? <LoadingMessage /> : <WeatherTable fc={forecasts}></WeatherTable>;

  return (
    <div>
      <h1 id="tabelLabel">Weather forecast</h1>
      <p>This component demonstrates fetching data from the server.</p>
      {contents}
    </div>
  );

  async function populateWeatherData() {
    const response = await fetch('weatherforecast');
    const data = await response.json();
    setForecasts(data);
  }
}

export default App;