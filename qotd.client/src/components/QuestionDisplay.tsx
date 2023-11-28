import { Container, Typography } from "@mui/material";

interface QuestionDisplayProps {
  question: string
}

function QuestionDisplay(props: QuestionDisplayProps) {

  return (
    <Container maxWidth="md">
      <Typography variant="h4" component="p" mt={2} mb={2}>
        {props.question}
      </Typography>
    </Container>
  );
}

export default QuestionDisplay;