import { Container, SwipeableDrawer } from "@mui/material";
//import { grey } from '@mui/material/colors';

interface AppDrawerProps {
  open: boolean,
  anchor: 'left' | 'top' | 'right' | 'bottom',
  children?: React.ReactNode,
  handleClose: () => void
}

AppDrawer.defaultProps = {
  anchor: 'top'
};

function AppDrawer(props: AppDrawerProps) {

  return (
    <SwipeableDrawer
      anchor={props.anchor}
      open={props.open}
      onOpen={props.handleClose}
      onClose={props.handleClose}
      disableBackdropTransition={true}
      disableDiscovery={true}
      disableSwipeToOpen={true}
    >
      <Container sx={{ /*backgroundColor: grey[800]*/ /*'text-wrap': 'pretty' */}}>
        {props.children}
      </Container>
    </SwipeableDrawer>
  );
}

export default AppDrawer;