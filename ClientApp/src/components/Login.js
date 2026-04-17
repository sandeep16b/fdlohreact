import React from 'react';
import { Container, Row, Col, Card, CardBody, Button } from 'reactstrap';

export function Login() {
  return (
    <Container fluid style={{ paddingTop: '50px' }}>
      <Row className="justify-content-center">
        <Col md="6">
          <Card>
            <CardBody style={{ textAlign: 'center', padding: '40px' }}>
              <h2>Asset Management System</h2>
              <p style={{ marginTop: '20px', fontSize: '16px' }}>
                Please log in to access the application
              </p>
              <Button
                color="primary"
                size="lg"
                onClick={() => window.location.href = '/account/login'}
                style={{ marginTop: '30px' }}
              >
                Sign In
              </Button>
            </CardBody>
          </Card>
        </Col>
      </Row>
    </Container>
  );
}
