package hr.antitalent.tanks_backend.controllers;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/test")
public class TestController {

    @RequestMapping("/hello")
    public String hello() {
        return "Hello, World!";
    }
}
