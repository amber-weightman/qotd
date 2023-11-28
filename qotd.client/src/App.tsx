import { Backdrop, Box, CircularProgress } from '@mui/material';
import { useEffect, useState } from 'react';
import './App.css';
import AppMenu from './components/AppMenu';
import QuestionDisplay from './components/QuestionDisplay';

function App() {
  const [question, setQuestion] = useState<string>();
  const [loading, setLoading] = useState(false);
  
  useEffect(() => {
    lookupQuestion();
  }, []);

  const lookupQuestion = async () => {
    setLoading(true);
    setQuestion(undefined);
    const response = await fetch('question');
    const data: string = await response.text();
    setQuestion(data);
    console.log(data);
    setLoading(false);
  }

  return (
    <Box>
        {question !== undefined && <QuestionDisplay question={question}></QuestionDisplay>}
        <Backdrop
          sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }}
          open={loading}
        >
          <CircularProgress color="inherit" />
        </Backdrop>
        <AppMenu question={question} lookupQuestion={lookupQuestion} />
      
      
    </Box>
  );
}

export default App;