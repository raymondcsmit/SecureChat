import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-error',
  templateUrl: './error.component.html',
  styleUrls: ['./error.component.css']
})
export class ErrorComponent implements OnInit {

  errors$: Observable<string[]>;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.errors$ = this.route.queryParamMap.pipe(
      map(params => JSON.parse(params.get('errors')))
    )
  }
}
