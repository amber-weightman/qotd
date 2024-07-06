import { Container, Divider, Link, Typography } from "@mui/material";

interface AboutProps {
  dividerTextAlign: 'left' | 'center' | 'right'
}

About.defaultProps = {
  dividerTextAlign: 'left'
};

function About(props: AboutProps) {

  return (
    <Container maxWidth="xl">
      <Divider textAlign={props.dividerTextAlign}>
        <Typography variant="h6" component="h2">About QOTD</Typography>
      </Divider>
      <Typography paragraph={true} variant={"body1"}>
        Just a little app for AI-generating questions of the day. Features .NET 8, React, Material UI, Azure and OpenAI.
      </Typography>
      <Typography paragraph={true} align="right">
        <Link href="https://questionoftheday.azurewebsites.net/swagger" target="_blank" rel="noopener">
          API
        </Link>
      </Typography>
      <Typography paragraph={true} align="right">
        <Link href="https://github.com/amber-weightman/qotd" target="_blank" rel="noopener">
          GitHub
        </Link>
      </Typography>
      
      <Divider textAlign={props.dividerTextAlign}>
        <Typography variant="h6" component="h2">About Amber</Typography>
      </Divider>
      <Typography paragraph={true} variant={"body1"}>
        I used to be a developer like you, but then I took a career-change to the knee.
      </Typography>

      <Typography paragraph={true} align="right">
        <Link href="https://www.linkedin.com/in/amberweightman" target="_blank" rel="noopener">
          LinkedIn
        </Link>
      </Typography>
    </Container>
  );
}

export default About;